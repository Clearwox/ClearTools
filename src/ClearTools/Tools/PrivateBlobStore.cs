using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace Clear.Tools {

public sealed class PrivateBlobStore
{
    private readonly BlobContainerClient container;

    public PrivateBlobStore(BlobContainerClient container) => this.container = container;

    public async Task<byte[]?> ReadAsync(string blobName, CancellationToken cancellationToken)
    {
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        var properties = await container.GetPropertiesAsync(cancellationToken: cancellationToken);
        if (properties.Value.PublicAccess != PublicAccessType.None)
            throw new InvalidOperationException("The blob container must be private.");
        try { return (await container.GetBlobClient(blobName).DownloadContentAsync(cancellationToken)).Value.Content.ToArray(); }
        catch (RequestFailedException ex) when (ex.Status == 404) { return null; }
    }

    public async Task WriteAsync(string blobName, byte[] bytes, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream(bytes, writable: false);
        await container.GetBlobClient(blobName).UploadAsync(stream,
            new BlobUploadOptions { Conditions = new BlobRequestConditions { IfNoneMatch = ETag.All } }, cancellationToken);
    }

    public async Task<IAsyncDisposable> AcquireAsync(string lockBlobName, CancellationToken cancellationToken)
    {
        var blob = container.GetBlobClient(lockBlobName);
        try
        {
            using var empty = new MemoryStream(Array.Empty<byte>());
            await blob.UploadAsync(empty, new BlobUploadOptions { Conditions = new BlobRequestConditions { IfNoneMatch = ETag.All } }, cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.Status == 409 || ex.Status == 412) { }

        var lease = blob.GetBlobLeaseClient();
        for (var attempt = 0; attempt < 30; attempt++)
        {
            try
            {
                await lease.AcquireAsync(TimeSpan.FromSeconds(60), cancellationToken: cancellationToken);
                return new RenewableLease(lease);
            }
            catch (RequestFailedException ex) when (ex.Status == 409)
            {
                if (attempt == 29) break;
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }
        throw new TimeoutException("Could not acquire the blob lease before the retry limit.");
    }

    private sealed class RenewableLease : IAsyncDisposable
    {
        private readonly BlobLeaseClient _lease;
        private readonly CancellationTokenSource _stop = new CancellationTokenSource();
        private readonly Task _renewal;

        public RenewableLease(BlobLeaseClient lease)
        {
            _lease = lease;
            _renewal = RenewAsync();
        }

        private async Task RenewAsync()
        {
            try
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), _stop.Token);
                    await _lease.RenewAsync(cancellationToken: _stop.Token);
                }
            }
            catch (OperationCanceledException) when (_stop.IsCancellationRequested) { }
        }

        public async ValueTask DisposeAsync()
        {
            _stop.Cancel();
            try { await _renewal; }
            finally
            {
                try { await _lease.ReleaseAsync(); }
                catch (RequestFailedException ex) when (ex.Status == 404 || ex.Status == 409 || ex.Status == 412) { }
                _stop.Dispose();
            }
        }
    }
}
}
