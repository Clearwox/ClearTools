using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Clear.Tools
{
    public static partial class AzureStorageManager
    {
        private static BlobContainerClient CreateCloudBlobContainer(string connectionString, string containerName) 
        => new BlobServiceClient(connectionString).GetBlobContainerClient(containerName);

        private static BlobClient GetBlobClient(string connectionString, string containerName, string fileName, string? folder)
        {
            var blobContainer = new BlobContainerClient(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            return blobContainer.GetBlobClient($"{folder}\\{fileName}".Trim('\\'));
        }

        public static bool AzureFolderExists(string connectionString, string containerName, string folder)
        {
            var blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            return blobContainer.GetBlobs(prefix: folder).Any();
        }

        #region upload to azure

        public static void UploadToAzure(string connectionString, string containerName, Stream stream,
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, fileName, folder).Upload(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public static void UploadToAzure(string connectionString, string containerName, MemoryStream stream,
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, fileName, folder).Upload(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public static void UploadToAzure(string connectionString, string containerName, FileStream stream,
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, fileName, folder).Upload(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public static void UploadToAzure(string connectionString, string containerName, FileInfo file,
            string contentType, string folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, file.Name, folder).Upload(
                file.FullName, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public static async Task UploadToAzureAsync(string connectionString, string containerName, Stream stream,
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, fileName, folder).UploadAsync(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public static async Task UploadToAzureAsync(string connectionString, string containerName, MemoryStream stream,
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, fileName, folder).UploadAsync(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public static async Task UploadToAzureAsync(string connectionString, string containerName, FileStream stream,
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, fileName, folder).UploadAsync(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public static async Task UploadToAzureAsync(string connectionString, string containerName, FileInfo file,
            string contentType, string folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, file.Name, folder).UploadAsync(
                file.FullName, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        #endregion

        #region download from azure

        public static void DownloadFromAzure(string connectionString, string containerName,
            FileInfo file, string? folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, file.Name, folder).DownloadTo(
                file.FullName, cancellationToken: cancellationToken
            );
        }

        public static void DownloadFromAzure(string connectionString, string containerName,
            FileStream file, string? folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, file.Name, folder).DownloadTo(
                file, cancellationToken: cancellationToken
            );
        }

        public static void DownloadFromAzure(string connectionString, string containerName,
            string filename, MemoryStream file, string? folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, filename, folder).DownloadTo(
                file, cancellationToken: cancellationToken
            );
        }

        public static async Task DownloadFromAzureAsync(string connectionString, string containerName,
            FileInfo file, string? folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, file.Name, folder).DownloadToAsync(
                file.FullName, cancellationToken: cancellationToken
            );
        }

        public static async Task DownloadFromAzureAsync(string connectionString, string containerName,
            FileStream file, string? folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, file.Name, folder).DownloadToAsync(
                file, cancellationToken: cancellationToken
            );
        }

        public static async Task DownloadFromAzureAsync(string connectionString, string containerName,
            string filename, MemoryStream file, string? folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, filename, folder).DownloadToAsync(
                file, cancellationToken: cancellationToken
            );
        }

        #endregion

        public static void DeleteFromAzure(string connectionString, string containerName, string fileName,
            string? folder, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            var blockBlob = blobContainer.GetBlobClient($"{folder}\\{fileName}".Trim('\\'));
            blockBlob.DeleteIfExists(cancellationToken: cancellationToken);
        }

        public static async Task DeleteFromAzureAsync(string connectionString, string containerName,
            string fileName, string? folder, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            var blockBlob = blobContainer.GetBlobClient($"{folder}\\{fileName}".Trim('\\'));
            await blockBlob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
    }
}