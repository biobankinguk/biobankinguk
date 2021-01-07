using System.Threading.Tasks;
using Entities.Api;
using Biobanks.SubmissionAzureFunction.Dtos;

namespace Biobanks.SubmissionAzureFunction.Services.Contracts
{
    public interface IDiagnosisValidationService
    {
        Task<Diagnosis> ValidateAndPopulateDiagnosis(DiagnosisDto dto, StagedDiagnosis diagnosis = null);
    }
}
