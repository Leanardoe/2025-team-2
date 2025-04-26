using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;

namespace ResumeSystem.Services
{
    public class BlobService
    {
        private readonly IConfiguration _config;
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(IConfiguration config)
        {
            _config = config;
            string connectionString = _config.GetConnectionString("AzureStorage");

            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        // Upload a resume and return the blob name
        public async Task<string> UploadResumeAsync(IFormFile file, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobName;
        }

        // Generate a secure download URL with SAS
        public string GenerateDownloadSasUri(string containerName, string blobName, TimeSpan expiry)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!blobClient.CanGenerateSasUri)
                throw new InvalidOperationException("BlobClient is not authorized to generate SAS URIs. Use key-based credentials.");

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        // Optionally delete a blob if needed
        public async Task DeleteResumeAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
