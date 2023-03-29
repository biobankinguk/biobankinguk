using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Submissions.Dto;

namespace Core.Submissions.Services.Contracts
{
    public interface ISampleWriteService
    {
        Task ProcessSamples(IEnumerable<SampleDto> dto);
        Task DeleteSamplesIfExists(IEnumerable<SampleIdDto> dto);
    }
}
