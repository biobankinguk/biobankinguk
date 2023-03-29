using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Core.Submissions.Dto;

namespace Core.Submissions.Services.Contracts
{
    public interface IDiagnosisValidationService
    {
        Task<Diagnosis> ValidateAndPopulateDiagnosis(DiagnosisDto dto, StagedDiagnosis diagnosis = null);
    }
}
