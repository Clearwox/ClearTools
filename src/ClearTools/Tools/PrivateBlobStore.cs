using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace Clear.Tools
{
    /// <summary>A renewable lease. Protected operations must observe OwnershipLost.</summary>
    public interface IBlobLease : IAsyncDisposable
    {
        CancellationToken OwnershipLost { get; }
    }

    public sealed class PrivateBlobStore
    {
        private readonly BlobContainerClient _container;

        public PrivateBlobStore(BlobContainerClient container) => _container = container;

        /// <summary>Explicit writer initialization. Reading existing blobs requires no create permission.</summary>
        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await _container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
            await EnsurePrivateAsync(cancellationToken);
        }

        private async Task EnsurePrivateAsync(CancellationToken cancellationToken)
        {
            var properties = await _container.GetPropertiesAsync(cancellationToken: cancellationToken);
            if (properties.Value.PublicAccess != PublicAccessType.None)
                throw new InvalidOperationException("The blob container must be private.");
        }

        public async Task<byte[]?> ReadAsync(string blobName, CancellationToken cancellationToken)
        {
            try
            {
                await EnsurePrivateAsync(cancellationToken);
                var response = await _container.GetBlobClient(blobName).DownloadContentAsync(cancellationToken);
                return response.Value.Content.ToArray();
            }
            catch (RequestFailedException ex) when (ex.Status == 404) { return null; }
        }

        public async Task WriteAsync(string blobName, byte[] bytes, CancellationToken cancellationToken)
        {
            await EnsurePrivateAsync(cancellationToken);
            using var stream = new MemoryStream(bytes, writable: false);
            await _container.GetBlobClient(blobName).UploadAsync(stream,
                new BlobUploadOptions { Conditions = new BlobRequestConditions { IfNoneMatch = ETag.All } }, cancellationToken);
        }

        public async Task<IBlobLease> AcquireAsync(string lockBlobName, CancellationToken cancellationToken)
        {
            await InitializeAsync(cancellationToken);
            var blob = _container.GetBlobClient(lockBlobName);
            try
            {
                using var empty = new MemoryStream(Array.Empty<byte>());
                await blob.UploadAsync(empty, new BlobUploadOptions
                {
                    Conditions = new BlobRequestConditions { IfNoneMatch = ETag.All }
                }, cancellationToken);
            }
            catch (RequestFailedException ex) when (ex.Status == 409 || ex.Status == 412) { }

            var lease = blob.GetBlobLeaseClient();
            for (var attempt = 0; attempt < 30; attempt++)
            {
                try
                {
                    using var acquisition = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    acquisition.CancelAfter(TimeSpan.FromSeconds(45));
                    var elapsed = Stopwatch.StartNew();
                    await lease.AcquireAsync(TimeSpan.FromSeconds(60), cancellationToken: acquisition.Token);
                    acquisition.Token.ThrowIfCancellationRequested();
                    var remaining = TimeSpan.FromSeconds(45) - elapsed.Elapsed;
                    if (remaining <= TimeSpan.Zero) throw new TimeoutException("Lease acquisition exceeded its ownership window.");
                    return new RenewableBlobLease(lease, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(45), remaining);
                }
                catch (RequestFailedException ex) when (ex.Status == 409)
                {
                    if (attempt == 29) break;
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                }
            }
            throw new TimeoutException("Could not acquire the blob lease before the retry limit.");
        }
    }

    internal sealed class RenewableBlobLease : IBlobLease
    {
        private readonly BlobLeaseClient _lease;
        private readonly CancellationTokenSource _stop = new CancellationTokenSource();
        private readonly CancellationTokenSource _lost = new CancellationTokenSource();
        private readonly Task _renewal;

        public CancellationToken OwnershipLost { get; }

        internal RenewableBlobLease(BlobLeaseClient lease, TimeSpan renewalInterval, TimeSpan ownershipWindow, TimeSpan? initialWindow = null)
        {
            _lease = lease;
            OwnershipLost = _lost.Token;
            // Fail closed before the server's 60-second lease expires, even if renewal hangs.
            _lost.CancelAfter(initialWindow ?? ownershipWindow);
            _renewal = RenewAsync(renewalInterval, ownershipWindow);
        }

        private async Task RenewAsync(TimeSpan interval, TimeSpan ownershipWindow)
        {
            using var renewalToken = CancellationTokenSource.CreateLinkedTokenSource(_stop.Token, OwnershipLost);
            try
            {
                while (true)
                {
                    await Task.Delay(interval, renewalToken.Token);
                    var elapsed = Stopwatch.StartNew();
                    await _lease.RenewAsync(cancellationToken: renewalToken.Token);
                    renewalToken.Token.ThrowIfCancellationRequested();
                    var remaining = ownershipWindow - elapsed.Elapsed;
                    if (remaining <= TimeSpan.Zero)
                    {
                        _lost.Cancel();
                        return;
                    }
                    _lost.CancelAfter(remaining);
                }
            }
            catch (OperationCanceledException) when (_stop.IsCancellationRequested) { }
            catch (Exception) { _lost.Cancel(); }
        }

        public async ValueTask DisposeAsync()
        {
            _stop.Cancel();
            try { await _renewal; }
            finally
            {
                using var releaseTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                try { await _lease.ReleaseAsync(cancellationToken: releaseTimeout.Token); }
                catch (RequestFailedException ex) when (ex.Status == 404 || ex.Status == 409 || ex.Status == 412) { }
                finally
                {
                    _lost.Dispose();
                    _stop.Dispose();
                }
            }
        }
    }
}
