using System.Threading.Tasks;

namespace Biobanks.Submissions.Core.Services.Contracts
{
    public interface IQueueWriteService
    {
        Task DeleteAsync(string queue, string messageId);
    }
}