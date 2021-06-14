using Azure.Storage.Blobs;
using Core.Submissions.Services.Contracts;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Core.AzureStorage
{
    /// <inheritdoc />
    public class AzureBlobReadService : IBlobReadService
    {
        private readonly BlobServiceClient _blobsClient;

        /// <inheritdoc />
        public AzureBlobReadService(string connectionString)
        {
            _blobsClient = new BlobServiceClient(connectionString);
        }

        /// <inheritdoc />
        public async Task<string> GetObjectFromJsonAsync(string container, Guid id)
        {
            string downloadedJson;
            var blobClient = _blobsClient.GetBlobContainerClient(container).GetBlobClient(id.ToString());

            using (var stream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(stream);
                downloadedJson = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }

            return downloadedJson;
        }
    }
}
