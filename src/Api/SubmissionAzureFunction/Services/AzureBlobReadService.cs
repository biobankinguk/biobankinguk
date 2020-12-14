using System;
using System.IO;
using System.Threading.Tasks;
using Biobanks.SubmissionAzureFunction.Services.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Biobanks.SubmissionAzureFunction.Services
{
    /// <inheritdoc />
    public class AzureBlobReadService : IBlobReadService
    {
        private readonly CloudBlobClient _blobClient;

        /// <inheritdoc />
        public AzureBlobReadService(CloudStorageAccount storageAccount)
        {
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        /// <inheritdoc />
        public async Task<string> GetObjectFromJsonAsync(string container, Guid id)
        {
            string downloadedJson;
            var blobBlock = _blobClient.GetContainerReference(container).GetBlobReference(id.ToString());

            using (var stream = new MemoryStream())
            {
                await blobBlock.DownloadToStreamAsync(stream);
                downloadedJson = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }

            return downloadedJson;
        }
    }
}
