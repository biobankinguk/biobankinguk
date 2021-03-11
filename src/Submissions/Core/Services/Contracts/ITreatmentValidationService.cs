using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Core.Dto;

namespace Biobanks.Submissions.Core.Services.Contracts
{
    public interface ITreatmentValidationService
    {
        Task<StagedTreatment> ValidateAndPopulateTreatment(TreatmentDto dto, StagedTreatment treatment = null);
    }
}
