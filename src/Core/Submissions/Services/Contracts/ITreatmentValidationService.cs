using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Core.Submissions.Dto;

namespace Core.Submissions.Services.Contracts
{
    public interface ITreatmentValidationService
    {
        Task<StagedTreatment> ValidateAndPopulateTreatment(TreatmentDto dto, StagedTreatment treatment = null);
    }
}
