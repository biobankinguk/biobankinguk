using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.SubmissionAzureFunction.Dtos;

namespace Biobanks.SubmissionAzureFunction.Services.Contracts
{
    public interface IDiagnosisWriteService
    {
        Task ProcessDiagnoses(IEnumerable<DiagnosisDto> dto);
        Task DeleteDiagnosesIfExists(IEnumerable<DiagnosisDto> dto);
    }
}
