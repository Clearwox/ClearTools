using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Clear
{
    public interface IFileManager
    {
        string ReadFile(string filename);
        Task<string> ReadFileAsync(string filename);
        void WriteToFile(string filename, string text);
        Task WriteToFileAsync(string filename, string text);

        bool AzureFolderExists(string connectionString, string containerName, string folder);
        
        void DeleteFromAzure(string connectionString, string containerName, string fileName, string folder, CancellationToken cancellationToken = default);
        Task DeleteFromAzureAsync(string connectionString, string containerName, string fileName, string folder, CancellationToken cancellationToken = default);
        
        void DownloadFromAzure(string connectionString, string containerName, FileInfo file, string folder, CancellationToken cancellationToken = default);
        void DownloadFromAzure(string connectionString, string containerName, FileStream file, string folder, CancellationToken cancellationToken = default);
        void DownloadFromAzure(string connectionString, string containerName, string filename, MemoryStream file, string folder, CancellationToken cancellationToken = default);
        
        Task DownloadFromAzureAsync(string connectionString, string containerName, FileInfo file, string folder, CancellationToken cancellationToken = default);
        Task DownloadFromAzureAsync(string connectionString, string containerName, FileStream file, string folder, CancellationToken cancellationToken = default);
        Task DownloadFromAzureAsync(string connectionString, string containerName, string filename, MemoryStream file, string folder, CancellationToken cancellationToken = default);
        
        void UploadToAzure(string connectionString, string containerName, Stream stream, string contentType, string fileName, string folder, CancellationToken cancellationToken = default);
        void UploadToAzure(string connectionString, string containerName, MemoryStream stream, string contentType, string fileName, string folder, CancellationToken cancellationToken = default);
        void UploadToAzure(string connectionString, string containerName, FileStream stream, string contentType, string fileName, string folder, CancellationToken cancellationToken = default);
        void UploadToAzure(string connectionString, string containerName, FileInfo file, string contentType, string folder, CancellationToken cancellationToken = default);
        
        Task UploadToAzureAsync(string connectionString, string containerName, Stream stream, string contentType, string fileName, string folder, CancellationToken cancellationToken = default);
        Task UploadToAzureAsync(string connectionString, string containerName, MemoryStream stream, string contentType, string fileName, string folder, CancellationToken cancellationToken = default);
        Task UploadToAzureAsync(string connectionString, string containerName, FileStream stream, string contentType, string fileName, string folder, CancellationToken cancellationToken = default);
        Task UploadToAzureAsync(string connectionString, string containerName, FileInfo file, string contentType, string folder, CancellationToken cancellationToken = default);
    }
    public class FileManager : IFileManager
    {
        #region local file

        public void WriteToFile(string filename, string text)
        {
            //Pass the filepath and filename to the StreamWriter Constructor
            using StreamWriter sw = new StreamWriter(filename);

            //Write a line of text
            sw.WriteLine(text);

            //Close the file
            sw.Close();
        }

        public string ReadFile(string filename)
        {
            string line = "";

            //Pass the file path and file name to the StreamReader constructor
            using (StreamReader sr = new StreamReader(filename))
            {
                //Read the first line of text
                line = sr.ReadToEnd();

                //close the file
                sr.Close();
            }

            return line;
        }

        public async Task WriteToFileAsync(string filename, string text)
        {
            //Pass the filepath and filename to the StreamWriter Constructor
            using (StreamWriter sw = new StreamWriter(filename))
            {
                //Write a line of text
                await sw.WriteLineAsync(text);

                //Close the file
                sw.Close();
            }
        }

        public async Task<string> ReadFileAsync(string filename)
        {
            string line = "";

            //Pass the file path and file name to the StreamReader constructor
            using (StreamReader sr = new StreamReader(filename))
            {
                //Read the first line of text
                line = await sr.ReadToEndAsync();

                //close the file
                sr.Close();
            }

            return line;
        }

        #endregion

        #region azure storage

        private BlobContainerClient CreateCloudBlobContainer(string connectionString, string containerName) =>
            new BlobServiceClient(connectionString).GetBlobContainerClient(containerName);

        private BlobClient GetBlobClient(string connectionString, string containerName, string fileName, string folder)
        {
            BlobContainerClient blobContainer = new BlobContainerClient(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            return blobContainer.GetBlobClient($"{folder}\\{fileName}".Trim('\\'));
        }

        public bool AzureFolderExists(string connectionString, string containerName, string folder)
        {
            BlobContainerClient blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            return blobContainer.GetBlobs(prefix: folder).Any();
        }

        #region upload to azure

        public void UploadToAzure(string connectionString, string containerName, Stream stream, 
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, fileName, folder).Upload(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public void UploadToAzure(string connectionString, string containerName, MemoryStream stream, 
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, fileName, folder).Upload(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public void UploadToAzure(string connectionString, string containerName, FileStream stream,
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, fileName, folder).Upload(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public void UploadToAzure(string connectionString, string containerName, FileInfo file,
            string contentType, string folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, file.Name, folder).Upload(
                file.FullName, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        // async

        public async Task UploadToAzureAsync(string connectionString, string containerName, Stream stream, 
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, fileName, folder).UploadAsync(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public async Task UploadToAzureAsync(string connectionString, string containerName, MemoryStream stream, 
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, fileName, folder).UploadAsync(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public async Task UploadToAzureAsync(string connectionString, string containerName, FileStream stream, 
            string contentType, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, fileName, folder).UploadAsync(
                stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        public async Task UploadToAzureAsync(string connectionString, string containerName, FileInfo file, 
            string contentType, string folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, file.Name, folder).UploadAsync(
                file.FullName, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken
            );
        }

        #endregion

        #region download from azure

        public void DownloadFromAzure(string connectionString, string containerName, 
            FileInfo file, string folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, file.Name, folder).DownloadTo(
                file.FullName, cancellationToken: cancellationToken
            );
        }

        public void DownloadFromAzure(string connectionString, string containerName, 
            FileStream file, string folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, file.Name, folder).DownloadTo(
                file, cancellationToken: cancellationToken
            );
        }

        public void DownloadFromAzure(string connectionString, string containerName, 
            string filename, MemoryStream file, string folder, CancellationToken cancellationToken = default)
        {
            GetBlobClient(connectionString, containerName, filename, folder).DownloadTo(
                file, cancellationToken: cancellationToken
            );
        }

        // async

        public async Task DownloadFromAzureAsync(string connectionString, string containerName, 
            FileInfo file, string folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, file.Name, folder).DownloadToAsync(
                file.FullName, cancellationToken: cancellationToken
            );
        }

        public async Task DownloadFromAzureAsync(string connectionString, string containerName, 
            FileStream file, string folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, file.Name, folder).DownloadToAsync(
                file, cancellationToken: cancellationToken
            );
        }

        public async Task DownloadFromAzureAsync(string connectionString, string containerName, 
            string filename, MemoryStream file, string folder, CancellationToken cancellationToken = default)
        {
            await GetBlobClient(connectionString, containerName, filename, folder).DownloadToAsync(
                file, cancellationToken: cancellationToken
            );
        }

        #endregion

        public void DeleteFromAzure(string connectionString, string containerName, string fileName, 
            string folder, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            var blockBlob = blobContainer.GetBlobClient($"{folder}\\{fileName}".Trim('\\'));
            blockBlob.DeleteIfExists(cancellationToken: cancellationToken);
        }

        public async Task DeleteFromAzureAsync(string connectionString, string containerName, 
            string fileName, string folder, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            var blockBlob = blobContainer.GetBlobClient($"{folder}\\{fileName}".Trim('\\'));
            await blockBlob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        #endregion
    }
}