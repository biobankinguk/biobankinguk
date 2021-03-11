using System.Threading.Tasks;
using Biobanks.Submissions.Api.Services.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Biobanks.Submissions.Api.Services
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
