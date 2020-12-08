using System.Threading.Tasks;
using Biobanks.Common.Data.Entities;
using Biobanks.SubmissionStagingJob.Dtos;

namespace Biobanks.SubmissionStagingJob.Services.Contracts
{
    public interface IDiagnosisValidationService
    {
        Task<Diagnosis> ValidateAndPopulateDiagnosis(DiagnosisDto dto, StagedDiagnosis diagnosis = null);
    }
}
