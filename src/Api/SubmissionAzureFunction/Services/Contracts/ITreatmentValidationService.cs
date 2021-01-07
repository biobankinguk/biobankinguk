using System.Threading.Tasks;
using Entities.Api;
using Biobanks.SubmissionAzureFunction.Dtos;

namespace Biobanks.SubmissionAzureFunction.Services.Contracts
{
    public interface ITreatmentValidationService
    {
        Task<StagedTreatment> ValidateAndPopulateTreatment(TreatmentDto dto, StagedTreatment treatment = null);
    }
}
