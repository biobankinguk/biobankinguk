using System;
using System.Threading.Tasks;

namespace Biobanks.SubmissionJob.Services.Contracts
{
    public interface IBlobWriteService
    {
        Task DeleteAsync(string container, Guid id);
    }
}
