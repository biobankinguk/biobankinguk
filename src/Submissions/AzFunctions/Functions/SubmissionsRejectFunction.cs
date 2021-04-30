using Biobanks.Submissions.Core.Models;
using Biobanks.Submissions.Core.Services.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzFunctions.Functions
{
    public class SubmissionsRejectFunction
    {
        //cloud services
        private readonly IQueueWriteService _queueWriteService;

        private readonly IRejectService _rejectService;

        public SubmissionsRejectFunction(IQueueWriteService queueWriteService, IRejectService rejectService)
        {
            _queueWriteService = queueWriteService;
            _rejectService = rejectService;
        }

        [Function("Submissions_Reject")]
        public async Task Run(
        [QueueTrigger("reject", Connection = "WorkerStorage")]
        string messageBody,
            FunctionContext context, // out of process context
                                     // QueueTrigger metadata
            string id,
            string popReceipt)
        {
            var log = context.GetLogger("Submissions_Reject");
            var message = JsonSerializer.Deserialize<RejectQueueItem>(messageBody);

            try
            {
                log.LogInformation($@"Starting Reject for Biobank: {message.BiobankId}");
                await _rejectService.RejectStagedData(message.BiobankId);
            }
            catch (Exception e)
            {
                log.LogInformation($@"Error when Rejecting Samples for Biobank:{message.BiobankId}: {e.Message}");
                throw;
            }

            await _queueWriteService.DeleteAsync("reject", id, popReceipt);

        }
    }
}
