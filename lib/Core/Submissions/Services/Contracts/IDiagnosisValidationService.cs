using System.Threading.Tasks;
using Biobanks.Data.Entities.Api;
using Biobanks.Submissions.Dto;

namespace Biobanks.Submissions.Services.Contracts
{
    public interface IDiagnosisValidationService
    {
        Task<Diagnosis> ValidateAndPopulateDiagnosis(DiagnosisDto dto, StagedDiagnosis diagnosis = null);
    }
}
