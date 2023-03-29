using System.Threading.Tasks;
using Azure.Storage.Queues;
using Biobanks.Submissions.Services.Contracts;

namespace Biobanks.AzureStorage
{
    /// <inheritdoc />
    public class AzureQueueWriteService : IQueueWriteService
    {
        private readonly string _connectionString;

        /// <inheritdoc />
        public AzureQueueWriteService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public async Task PushAsync(string queue, string message)
        {
            var q = new QueueClient(_connectionString, queue, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });

            await q.CreateIfNotExistsAsync();
            await q.SendMessageAsync(message);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string queue, string messageId, string popReceipt)
        {
            var q = new QueueClient(_connectionString, queue, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });

            await q.DeleteMessageAsync(messageId, popReceipt);
        }
    }
}
