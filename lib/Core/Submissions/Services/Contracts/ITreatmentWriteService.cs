using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Submissions.Dto;

namespace Biobanks.Submissions.Services.Contracts
{
    public interface ITreatmentWriteService
    {
        Task ProcessTreatments(IEnumerable<TreatmentDto> dto);
        Task DeleteTreatmentsIfExists(IEnumerable<TreatmentIdDto> dto);
    }
}
