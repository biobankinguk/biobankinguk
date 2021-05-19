using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Submissions.Dto;

namespace Core.Submissions.Services.Contracts
{
    public interface IDiagnosisWriteService
    {
        Task ProcessDiagnoses(IEnumerable<DiagnosisDto> dto);
        Task DeleteDiagnosesIfExists(IEnumerable<DiagnosisDto> dto);
    }
}
