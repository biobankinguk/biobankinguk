using Biobanks.Submissions.Core.Models;
using Biobanks.Submissions.Core.Services.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzFunctions.Functions
{
    public class SubmissionCommitFunction
    {
        //Cloud Services
        private readonly IQueueWriteService _queueWriteService;

        private readonly ICommitService _commitService;

        public SubmissionCommitFunction(IQueueWriteService queueWriteService, ICommitService commitService)
        {
            _queueWriteService = queueWriteService;
            _commitService = commitService;
        }

        [Function("Submissions_Commit")]
        public async Task Run(
        [QueueTrigger("commits", Connection = "WorkerStorage")]
        string messageBody,
            FunctionContext context, // out of process context
                                     // QueueTrigger metadata
            string id,
            string popReceipt)
        {
            var log = context.GetLogger("Submissions_Commit");
            var message = JsonSerializer.Deserialize<CommitQueueItem>(messageBody);

            try
            {
                log.LogInformation($@"Starting Commit for Biobank: {message.BiobankId}");
                await _commitService.CommitStagedData(message.Replace, message.BiobankId);
            }
            catch(Exception e)
            {
                log.LogInformation($@"Error when Committing Samples for Biobank:{message.BiobankId}: {e.Message}");
                throw;
            }

            await _queueWriteService.DeleteAsync("commits", id, popReceipt);
        }
    }
}
