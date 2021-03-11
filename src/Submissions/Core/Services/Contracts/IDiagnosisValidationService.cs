using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Core.Dto;

namespace Biobanks.Submissions.Core.Services.Contracts
{
    public interface IDiagnosisValidationService
    {
        Task<Diagnosis> ValidateAndPopulateDiagnosis(DiagnosisDto dto, StagedDiagnosis diagnosis = null);
    }
}
