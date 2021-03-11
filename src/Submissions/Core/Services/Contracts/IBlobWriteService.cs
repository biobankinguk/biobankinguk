using System;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Core.Services.Contracts
{
    public interface IBlobWriteService
    {
        Task DeleteAsync(string container, Guid id);
    }
}
