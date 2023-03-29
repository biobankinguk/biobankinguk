using System.Threading.Tasks;
using Biobanks.Data.Entities.Shared.ReferenceData;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface IDiseaseStatusService
    {
        Task<SnomedTag> GetSnomedTagByDescription(string description);
    }
}
