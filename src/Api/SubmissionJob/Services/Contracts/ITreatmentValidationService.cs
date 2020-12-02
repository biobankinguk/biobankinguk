using System.Threading.Tasks;
using Biobanks.Common.Data.Entities;
using Biobanks.SubmissionJob.Dtos;

namespace Biobanks.SubmissionJob.Services.Contracts
{
    public interface ITreatmentValidationService
    {
        Task<StagedTreatment> ValidateAndPopulateTreatment(TreatmentDto dto, StagedTreatment treatment = null);
    }
}
