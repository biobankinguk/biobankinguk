using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.SubmissionStagingJob.Dtos;

namespace Biobanks.SubmissionStagingJob.Services.Contracts
{
    public interface ITreatmentWriteService
    {
        Task ProcessTreatments(IEnumerable<TreatmentDto> dto);
        Task DeleteTreatmentsIfExists(IEnumerable<TreatmentIdDto> dto);
    }
}
