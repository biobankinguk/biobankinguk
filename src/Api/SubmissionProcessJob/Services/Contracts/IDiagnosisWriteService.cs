using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.SubmissionProcessJob.Dtos;

namespace Biobanks.SubmissionProcessJob.Services.Contracts
{
    public interface IDiagnosisWriteService
    {
        Task ProcessDiagnoses(IEnumerable<DiagnosisDto> dto);
        Task DeleteDiagnosesIfExists(IEnumerable<DiagnosisDto> dto);
    }
}
