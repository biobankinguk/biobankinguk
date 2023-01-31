using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Core.Submissions.Services.Contracts;

namespace Core.AzureStorage
{
    /// <inheritdoc />
    public class AzureBlobWriteService : IBlobWriteService
    {
        private readonly BlobServiceClient _blobsClient;

        /// <inheritdoc />
        public AzureBlobWriteService(string connectionString)
        {
            _blobsClient = new BlobServiceClient(connectionString);
        }

        /// <inheritdoc />
        public async Task<Guid> StoreObjectAsJsonAsync(string container, object obj)
        {
            var json = JsonSerializer.Serialize(obj);
            return await StoreTextAsync(container, json, "application/json");
        }

        /// <inheritdoc />
        public async Task<Guid> StoreTextAsync(string container, string text, string contentType = "text/plain")
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));

            var containerClient = _blobsClient.GetBlobContainerClient(container);

            if (!await containerClient.ExistsAsync())
                await containerClient.CreateAsync();

            var id = Guid.NewGuid(); // generate a unique id for the blob

            var blobClient = containerClient.GetBlobClient(id.ToString());

            await blobClient.UploadAsync(
                new MemoryStream(Encoding.UTF8.GetBytes(text)),
                new BlobHttpHeaders()
                {
                    ContentType = contentType
                });

            return id;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string container, Guid id)
        {
            var containerClient = _blobsClient.GetBlobContainerClient(container);
            await containerClient.GetBlobClient(id.ToString()).DeleteIfExistsAsync();
        }
    }
}
