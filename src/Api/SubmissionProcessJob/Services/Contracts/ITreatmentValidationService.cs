using System.Threading.Tasks;
using Biobanks.Common.Data.Entities;
using Biobanks.SubmissionProcessJob.Dtos;

namespace Biobanks.SubmissionProcessJob.Services.Contracts
{
    public interface ITreatmentValidationService
    {
        Task<StagedTreatment> ValidateAndPopulateTreatment(TreatmentDto dto, StagedTreatment treatment = null);
    }
}
