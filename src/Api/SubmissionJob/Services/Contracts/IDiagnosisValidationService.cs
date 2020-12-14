using System.Threading.Tasks;
using Biobanks.Common.Data.Entities;
using Biobanks.SubmissionJob.Dtos;

namespace Biobanks.SubmissionJob.Services.Contracts
{
    public interface IDiagnosisValidationService
    {
        Task<Diagnosis> ValidateAndPopulateDiagnosis(DiagnosisDto dto, StagedDiagnosis diagnosis = null);
    }
}
