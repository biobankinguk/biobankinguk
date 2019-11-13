using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using Upload.Contracts;

namespace Upload.Azure
{
    /// <inheritdoc />
    public class AzureQueueWriteService : IQueueWriteService
    {
        private readonly CloudQueueClient _queueClient;

        /// <inheritdoc />
        public AzureQueueWriteService(IConfiguration _config)
        {
            var storageAccount = CloudStorageAccount.Parse(
                            _config.GetConnectionString("AzureStorageAccount"));
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
