using System.Threading.Tasks;
using Biobanks.SubmissionJob.Services.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Biobanks.SubmissionJob.Services
{
    public class AzureQueueWriteService : IQueueWriteService
    {
        private readonly CloudQueueClient _queueClient;

        public AzureQueueWriteService(CloudStorageAccount storageAccount)
        {
            _queueClient = storageAccount.CreateCloudQueueClient();
        }

        public async Task DeleteAsync(string queue, CloudQueueMessage message)
        {
            var q = _queueClient.GetQueueReference(queue);
            await q.DeleteMessageAsync(message);
        }
    }
}
