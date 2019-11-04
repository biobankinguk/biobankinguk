using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;
using Upload.Contracts;

namespace Upload.Azure
{
    /// <inheritdoc />
    public class AzureBlobWriteService : IBlobWriteService
    {
        private readonly CloudBlobClient _blobClient;

        /// <inheritdoc />
        public AzureBlobWriteService(CloudStorageAccount storageAccount)
        {
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        /// <inheritdoc />
        public async Task<Guid> StoreObjectAsJsonAsync(string container, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return await StoreTextAsync(container, json, "application/json");
        }

        /// <inheritdoc />
        public async Task<Guid> StoreTextAsync(string container, string text, string contentType = "text/plain")
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));

            var blobContainer = _blobClient.GetContainerReference(container);

            if (!await blobContainer.ExistsAsync())
                await blobContainer.CreateAsync();

            var id = Guid.NewGuid(); // generate a unique id for the blob

            var blockBlob = blobContainer.GetBlockBlobReference(id.ToString());

            blockBlob.Properties.ContentType = contentType;

            await blockBlob.UploadTextAsync(text);

            return id;
        }
    }
}
