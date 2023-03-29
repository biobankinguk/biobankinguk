using System.Threading.Tasks;
using Biobanks.Data.Entities.Api;
using Biobanks.Submissions.Dto;

namespace Biobanks.Submissions.Services.Contracts
{
    public interface ITreatmentValidationService
    {
        Task<StagedTreatment> ValidateAndPopulateTreatment(TreatmentDto dto, StagedTreatment treatment = null);
    }
}
