using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Submissions.Dto;

namespace Biobanks.Submissions.Services.Contracts
{
    public interface IDiagnosisWriteService
    {
        Task ProcessDiagnoses(IEnumerable<DiagnosisDto> dto);
        Task DeleteDiagnosesIfExists(IEnumerable<DiagnosisDto> dto);
    }
}
