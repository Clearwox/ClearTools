﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Clear
{
    public interface IFileManager
    {
        bool AzureFolderExists(string connectionString, string containerName, string folder);
        void DeleteFromAzure(string connectionString, string containerName, string fileName, string folder);
        Task DeleteFromAzureAsync(string connectionString, string containerName, string fileName, string folder);
        void DownloadFromAzure(string connectionString, string containerName, FileInfo file, string folder);
        void DownloadFromAzure(string connectionString, string containerName, FileStream file, string folder);
        void DownloadFromAzure(string connectionString, string containerName, string filename, MemoryStream file, string folder);
        Task DownloadFromAzureAsync(string connectionString, string containerName, FileInfo file, string folder);
        Task DownloadFromAzureAsync(string connectionString, string containerName, FileStream file, string folder);
        Task DownloadFromAzureAsync(string connectionString, string containerName, string filename, MemoryStream file, string folder);
        string ReadFile(string filename);
        Task<string> ReadFileAsync(string filename);
        void UploadToAzure(string connectionString, string containerName, MemoryStream stream, string contentType, string fileName, string folder);
        void UploadToAzure(string connectionString, string containerName, FileStream stream, string contentType, string fileName, string folder);
        void UploadToAzure(string connectionString, string containerName, FileInfo file, string contentType, string folder);
        Task UploadToAzureAsync(string connectionString, string containerName, MemoryStream stream, string contentType, string fileName, string folder);
        Task UploadToAzureAsync(string connectionString, string containerName, FileStream stream, string contentType, string fileName, string folder);
        Task UploadToAzureAsync(string connectionString, string containerName, FileInfo file, string contentType, string folder);
        void WriteToFile(string filename, string text);
        Task WriteToFileAsync(string filename, string text);
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

        //public bool AzureFolderExists(string connectionString, string containerName, string folder)
        //{
        //    CloudBlobContainer blobContainer = CreateCloudBlobContainer(connectionString, containerName);
        //    blobContainer.CreateIfNotExists();
        //    var blockBlob = blobContainer.GetDirectoryReference(folder);
        //    return blockBlob.ListBlobs().Count() > 0;
        //}

        public bool AzureFolderExists(string connectionString, string containerName, string folder)
        {
            BlobContainerClient blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            return blobContainer.GetBlobs(prefix: folder).Any();
        }

        private BlobClient GetBlobClient(string connectionString, string containerName, string fileName, string folder)
        {
            BlobContainerClient blobContainer = new BlobContainerClient(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            return blobContainer.GetBlobClient($"{folder}\\{fileName}".Trim('\\'));
        }

        #region upload to azure

        public void UploadToAzure(string connectionString, string containerName, MemoryStream stream, string contentType, string fileName, string folder) =>
            GetBlobClient(connectionString, containerName, fileName, folder).Upload(stream, new BlobHttpHeaders { ContentType = contentType });

        public void UploadToAzure(string connectionString, string containerName, FileStream stream, string contentType, string fileName, string folder) =>
            GetBlobClient(connectionString, containerName, fileName, folder).Upload(stream, new BlobHttpHeaders { ContentType = contentType });

        public void UploadToAzure(string connectionString, string containerName, FileInfo file, string contentType, string folder) =>
            GetBlobClient(connectionString, containerName, file.Name, folder).Upload(file.FullName, new BlobHttpHeaders { ContentType = contentType });

        // async

        public async Task UploadToAzureAsync(string connectionString, string containerName, MemoryStream stream, string contentType, string fileName, string folder) =>
            await GetBlobClient(connectionString, containerName, fileName, folder).UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });

        public async Task UploadToAzureAsync(string connectionString, string containerName, FileStream stream, string contentType, string fileName, string folder) =>
            await GetBlobClient(connectionString, containerName, fileName, folder).UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });

        public async Task UploadToAzureAsync(string connectionString, string containerName, FileInfo file, string contentType, string folder) =>
            await GetBlobClient(connectionString, containerName, file.Name, folder).UploadAsync(file.FullName, new BlobHttpHeaders { ContentType = contentType });

        #endregion

        #region download from azure

        //public void DownloadFromAzure(string connectionString, string containerName, FileInfo file, string folder) =>
        //    CreateCloudBlobContainer(connectionString, containerName)
        //        .GetBlockBlobReference($"{folder}\\{file.Name}".Trim('\\'))
        //        .DownloadToFile(file.FullName, FileMode.OpenOrCreate);

        //public void DownloadFromAzure(string connectionString, string containerName, FileStream file, string folder) =>
        //    CreateCloudBlobContainer(connectionString, containerName)
        //        .GetBlockBlobReference($"{folder}\\{file.Name}".Trim('\\'))
        //        .DownloadToStream(file);

        //public void DownloadFromAzure(string connectionString, string containerName, string filename, MemoryStream file, string folder) =>
        //    CreateCloudBlobContainer(connectionString, containerName)
        //        .GetBlockBlobReference($"{folder}\\{filename}".Trim('\\'))
        //        .DownloadToStream(file);

        public void DownloadFromAzure(string connectionString, string containerName, FileInfo file, string folder) =>
            GetBlobClient(connectionString, containerName, file.Name, folder).DownloadTo(file.FullName);

        public void DownloadFromAzure(string connectionString, string containerName, FileStream file, string folder) =>
            GetBlobClient(connectionString, containerName, file.Name, folder).DownloadTo(file);

        public void DownloadFromAzure(string connectionString, string containerName, string filename, MemoryStream file, string folder) =>
            GetBlobClient(connectionString, containerName, filename, folder).DownloadTo(file);

        // async

        public async Task DownloadFromAzureAsync(string connectionString, string containerName, FileInfo file, string folder) =>
            await GetBlobClient(connectionString, containerName, file.Name, folder).DownloadToAsync(file.FullName);

        public async Task DownloadFromAzureAsync(string connectionString, string containerName, FileStream file, string folder) =>
            await GetBlobClient(connectionString, containerName, file.Name, folder).DownloadToAsync(file);

        public async Task DownloadFromAzureAsync(string connectionString, string containerName, string filename, MemoryStream file, string folder) =>
            await GetBlobClient(connectionString, containerName, filename, folder).DownloadToAsync(file);

        #endregion

        public void DeleteFromAzure(string connectionString, string containerName, string fileName, string folder)
        {
            BlobContainerClient blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            var blockBlob = blobContainer.GetBlobClient($"{folder}\\{fileName}".Trim('\\'));
            blockBlob.DeleteIfExists();
        }

        public async Task DeleteFromAzureAsync(string connectionString, string containerName, string fileName, string folder)
        {
            BlobContainerClient blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            var blockBlob = blobContainer.GetBlobClient($"{folder}\\{fileName}".Trim('\\'));
            await blockBlob.DeleteIfExistsAsync();
        }

        #endregion
    }
}