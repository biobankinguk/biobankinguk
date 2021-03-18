using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Submissions.Core.Dto;

namespace Biobanks.Submissions.Core.Services.Contracts
{
    public interface ITreatmentWriteService
    {
        Task ProcessTreatments(IEnumerable<TreatmentDto> dto);
        Task DeleteTreatmentsIfExists(IEnumerable<TreatmentIdDto> dto);
    }
}
