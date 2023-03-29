using Biobanks.Entities.Shared.ReferenceData;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IDiseaseStatusService
    {
        Task<SnomedTag> GetSnomedTagByDescription(string description);
    }
}
