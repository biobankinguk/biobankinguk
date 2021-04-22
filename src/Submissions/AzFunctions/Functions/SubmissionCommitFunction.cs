using Microsoft.Azure.Functions.Worker;
using System.Threading.Tasks;

namespace AzFunctions.Functions
{
    class SubmissionCommitFunction
    {

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


        }
    }
}
