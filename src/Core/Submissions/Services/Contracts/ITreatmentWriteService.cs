using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Submissions.Dto;

namespace Core.Submissions.Services.Contracts
{
    public interface ITreatmentWriteService
    {
        Task ProcessTreatments(IEnumerable<TreatmentDto> dto);
        Task DeleteTreatmentsIfExists(IEnumerable<TreatmentIdDto> dto);
    }
}
