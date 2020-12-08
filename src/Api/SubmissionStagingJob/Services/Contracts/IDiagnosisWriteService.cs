using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.SubmissionStagingJob.Dtos;

namespace Biobanks.SubmissionStagingJob.Services.Contracts
{
    public interface IDiagnosisWriteService
    {
        Task ProcessDiagnoses(IEnumerable<DiagnosisDto> dto);
        Task DeleteDiagnosesIfExists(IEnumerable<DiagnosisDto> dto);
    }
}
