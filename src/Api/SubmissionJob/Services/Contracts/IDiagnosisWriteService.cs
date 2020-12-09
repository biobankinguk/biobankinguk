using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.SubmissionJob.Dtos;

namespace Biobanks.SubmissionJob.Services.Contracts
{
    public interface IDiagnosisWriteService
    {
        Task ProcessDiagnoses(IEnumerable<DiagnosisDto> dto);
        Task DeleteDiagnosesIfExists(IEnumerable<DiagnosisDto> dto);
    }
}
