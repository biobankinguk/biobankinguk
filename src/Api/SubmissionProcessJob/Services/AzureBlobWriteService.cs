using System;
using System.Threading.Tasks;
using Biobanks.SubmissionProcessJob.Services.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Biobanks.SubmissionJob.Services
{
    public class AzureBlobWriteService : IBlobWriteService
    {
        private readonly CloudBlobClient _blobClient;

        public AzureBlobWriteService(CloudStorageAccount storageAccount)
        {
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task DeleteAsync(string container, Guid id)
        {
            var blobContainer = _blobClient.GetContainerReference(container);
            await blobContainer.GetBlobReference(id.ToString()).DeleteIfExistsAsync();
        }
    }
}
