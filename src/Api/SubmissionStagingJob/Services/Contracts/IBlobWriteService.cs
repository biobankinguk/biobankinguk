using System;
using System.Threading.Tasks;

namespace Biobanks.SubmissionAzureFunction.Services.Contracts
{
    public interface IBlobWriteService
    {
        Task DeleteAsync(string container, Guid id);
    }
}
