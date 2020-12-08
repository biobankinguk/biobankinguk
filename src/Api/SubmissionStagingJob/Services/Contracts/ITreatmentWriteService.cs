using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.SubmissionAzureFunction.Dtos;

namespace Biobanks.SubmissionAzureFunction.Services.Contracts
{
    public interface ITreatmentWriteService
    {
        Task ProcessTreatments(IEnumerable<TreatmentDto> dto);
        Task DeleteTreatmentsIfExists(IEnumerable<TreatmentIdDto> dto);
    }
}
