using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Clear
{
    public interface IFileManager
    {
        bool AzureFolderExists(string connectionString, string containerName, string folder);
        void DeleteFromAzure(string connectionString, string containerName, string fileName, string folder);
        void DownloadFromAzure(string connectionString, string containerName, FileInfo file, string folder);
        void DownloadFromAzure(string connectionString, string containerName, FileStream file, string folder);
        void DownloadFromAzure(string connectionString, string containerName, string filename, MemoryStream file, string folder);
        Task DownloadFromAzureAsync(string connectionString, string containerName, FileInfo file, string folder);
        Task DownloadFromAzureAsync(string connectionString, string containerName, FileStream file, string folder);
        Task DownloadFromAzureAsync(string connectionString, string containerName, string filename, MemoryStream file, string folder);
        string ReadFile(string filename);
        Task<string> ReadFileAsync(string filename);
        Task UploadToAzure(string connectionString, string containerName, FileInfo file, string contentType, string folder);
        Task UploadToAzure(string connectionString, string containerName, FileStream stream, string contentType, string fileName, string folder);
        Task UploadToAzure(string connectionString, string containerName, MemoryStream stream, string contentType, string fileName, string folder);
        void UploadToAzureAsync(string connectionString, string containerName, FileInfo file, string contentType, string folder);
        void UploadToAzureAsync(string connectionString, string containerName, FileStream stream, string contentType, string fileName, string folder);
        void UploadToAzureAsync(string connectionString, string containerName, MemoryStream stream, string contentType, string fileName, string folder);
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

        private CloudBlobContainer CreateCloudBlobContainer(string connectionString, string containerName) =>
            CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient().GetContainerReference(containerName);

        public bool AzureFolderExists(string connectionString, string containerName, string folder)
        {
            CloudBlobContainer blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            var blockBlob = blobContainer.GetDirectoryReference(folder);
            return blockBlob.ListBlobs().Count() > 0;
        }

        #region upload to azure

        public void UploadToAzureAsync(string connectionString, string containerName, MemoryStream stream, string contentType, string fileName, string folder)
        {
            CloudBlobContainer blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            var blockBlob = blobContainer.GetBlockBlobReference($"{folder}\\{fileName}".Trim('\\'));
            blockBlob.Properties.ContentType = contentType;
            blockBlob.UploadFromStream(stream, stream.Length);
        }

        public void UploadToAzureAsync(string connectionString, string containerName, FileStream stream, string contentType, string fileName, string folder)
        {
            CloudBlobContainer blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            var blockBlob = blobContainer.GetBlockBlobReference($"{folder}\\{fileName}".Trim('\\'));
            blockBlob.Properties.ContentType = contentType;
            blockBlob.UploadFromStream(stream, stream.Length);
        }

        public void UploadToAzureAsync(string connectionString, string containerName, FileInfo file, string contentType, string folder)
        {
            CloudBlobContainer blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            var blockBlob = blobContainer.GetBlockBlobReference($"{folder}\\{file.Name}".Trim('\\'));
            blockBlob.Properties.ContentType = contentType;
            blockBlob.UploadFromFile(file.FullName);
        }

        // async

        public async Task UploadToAzure(string connectionString, string containerName, MemoryStream stream, string contentType, string fileName, string folder)
        {
            CloudBlobContainer blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            var blockBlob = blobContainer.GetBlockBlobReference($"{folder}\\{fileName}".Trim('\\'));
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromStreamAsync(stream, stream.Length);
        }

        public async Task UploadToAzure(string connectionString, string containerName, FileStream stream, string contentType, string fileName, string folder)
        {
            CloudBlobContainer blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            var blockBlob = blobContainer.GetBlockBlobReference($"{folder}\\{fileName}".Trim('\\'));
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromStreamAsync(stream, stream.Length);
        }

        public async Task UploadToAzure(string connectionString, string containerName, FileInfo file, string contentType, string folder)
        {
            CloudBlobContainer blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            blobContainer.CreateIfNotExists();
            var blockBlob = blobContainer.GetBlockBlobReference($"{folder}\\{file.Name}".Trim('\\'));
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromFileAsync(file.FullName);
        }

        #endregion

        #region download from azure

        public void DownloadFromAzure(string connectionString, string containerName, FileInfo file, string folder) =>
            CreateCloudBlobContainer(connectionString, containerName)
                .GetBlockBlobReference($"{folder}\\{file.Name}".Trim('\\'))
                .DownloadToFile(file.FullName, FileMode.OpenOrCreate);

        public void DownloadFromAzure(string connectionString, string containerName, FileStream file, string folder) =>
            CreateCloudBlobContainer(connectionString, containerName)
                .GetBlockBlobReference($"{folder}\\{file.Name}".Trim('\\'))
                .DownloadToStream(file);

        public void DownloadFromAzure(string connectionString, string containerName, string filename, MemoryStream file, string folder) =>
            CreateCloudBlobContainer(connectionString, containerName)
                .GetBlockBlobReference($"{folder}\\{filename}".Trim('\\'))
                .DownloadToStream(file);

        // async

        public async Task DownloadFromAzureAsync(string connectionString, string containerName, FileInfo file, string folder) =>
            await CreateCloudBlobContainer(connectionString, containerName)
                .GetBlockBlobReference($"{folder}\\{file.Name}".Trim('\\'))
                .DownloadToFileAsync(file.FullName, FileMode.OpenOrCreate);

        public async Task DownloadFromAzureAsync(string connectionString, string containerName, FileStream file, string folder) =>
            await CreateCloudBlobContainer(connectionString, containerName)
                .GetBlockBlobReference($"{folder}\\{file.Name}".Trim('\\'))
                .DownloadToStreamAsync(file);

        public async Task DownloadFromAzureAsync(string connectionString, string containerName, string filename, MemoryStream file, string folder) =>
            await CreateCloudBlobContainer(connectionString, containerName)
                .GetBlockBlobReference($"{folder}\\{filename}".Trim('\\'))
                .DownloadToStreamAsync(file);

        #endregion

        public void DeleteFromAzure(string connectionString, string containerName, string fileName, string folder)
        {
            CloudBlobContainer blobContainer = CreateCloudBlobContainer(connectionString, containerName);
            var blockBlob = blobContainer.GetBlockBlobReference($"{folder}\\{fileName}".Trim('\\'));
            blockBlob.DeleteIfExists();
        }

        #endregion
    }
}