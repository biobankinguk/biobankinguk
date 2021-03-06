﻿using System.Threading.Tasks;
using Biobanks.SubmissionAzureFunction.Services.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Biobanks.SubmissionAzureFunction.Services
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
