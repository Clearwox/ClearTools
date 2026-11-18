using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Clear.Tools;
using Moq;

namespace ClearTools.Tests;

public sealed class PrivateBlobStoreTests
{
    [Fact]
    public async Task WriteRejectsPublicContainerBeforeAccessingBlob()
    {
        var container = Container(PublicAccessType.Blob);
        var store = new PrivateBlobStore(container.Object);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.WriteAsync("report", [1], default));
        container.Verify(c => c.GetBlobClient(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ReadRequiresOnlyPropertiesAndDownloadPermissions()
    {
        var container = Container(PublicAccessType.None);
        var blob = new Mock<BlobClient>(MockBehavior.Strict);
        container.Setup(c => c.GetBlobClient("report")).Returns(blob.Object);
        blob.Setup(b => b.DownloadContentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobDownloadResult(new BinaryData(new byte[] { 1, 2 })), Mock.Of<Response>()));
        var bytes = await new PrivateBlobStore(container.Object).ReadAsync("report", default);
        Assert.Equal(new byte[] { 1, 2 }, bytes);
        container.Verify(c => c.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), It.IsAny<IDictionary<string, string>>(),
            It.IsAny<BlobContainerEncryptionScopeOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WriteChecksPrivacyAndPreservesCreateOnlyCondition()
    {
        var container = Container(PublicAccessType.None);
        var blob = new Mock<BlobClient>();
        container.Setup(c => c.GetBlobClient("report")).Returns(blob.Object);
        await new PrivateBlobStore(container.Object).WriteAsync("report", [1], default);
        blob.Verify(b => b.UploadAsync(It.IsAny<Stream>(),
            It.Is<BlobUploadOptions>(options => options.Conditions.IfNoneMatch == ETag.All),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FailedRenewalCancelsProtectedOperationImmediately()
    {
        var client = new Mock<BlobLeaseClient>();
        client.Setup(c => c.RenewAsync(It.IsAny<RequestConditions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(503, "Unavailable"));
        await using var lease = new RenewableBlobLease(client.Object, TimeSpan.FromMilliseconds(10), TimeSpan.FromSeconds(2));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Task.Delay(TimeSpan.FromSeconds(5), lease.OwnershipLost));
        client.Verify(c => c.RenewAsync(It.IsAny<RequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HungRenewalCancelsBeforeOwnershipDeadline()
    {
        var client = new Mock<BlobLeaseClient>();
        client.Setup(c => c.RenewAsync(It.IsAny<RequestConditions>(), It.IsAny<CancellationToken>()))
            .Returns(async (RequestConditions _, CancellationToken token) =>
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
                return (Response<BlobLease>)null!;
            });
        await using var lease = new RenewableBlobLease(client.Object, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(100));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Task.Delay(TimeSpan.FromSeconds(5), lease.OwnershipLost));
    }

    private static Mock<BlobContainerClient> Container(PublicAccessType access)
    {
        var container = new Mock<BlobContainerClient>(MockBehavior.Strict);
        container.Setup(c => c.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobContainerProperties(default, default, default(LeaseStatus?), default(LeaseState?), default(LeaseDurationType?), access, null, null, null), Mock.Of<Response>()));
        return container;
    }
}
