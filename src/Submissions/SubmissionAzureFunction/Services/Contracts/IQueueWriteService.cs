using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Biobanks.SubmissionAzureFunction.Services.Contracts
{
    public interface IQueueWriteService
    {
        Task DeleteAsync(string queue, CloudQueueMessage message);
    }
}