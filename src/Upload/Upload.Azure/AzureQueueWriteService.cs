using System.Threading.Tasks;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace Upload.Azure
{
    /// <inheritdoc />
    public class AzureQueueWriteService : IQueueWriteService
    {
        private readonly CloudQueueClient _queueClient;

        /// <inheritdoc />
        public AzureQueueWriteService(CloudStorageAccount storageAccount)
        {
            _queueClient = storageAccount.CreateCloudQueueClient();
        }

        /// <inheritdoc />
        public async Task PushAsync(string queue, string message)
        {
            var q = _queueClient.GetQueueReference(queue);
            await q.CreateIfNotExistsAsync();
            await q.AddMessageAsync(new CloudQueueMessage(message));
        }
    }
}
