using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Submissions.Dto;

namespace Biobanks.Submissions.Services.Contracts
{
    public interface ISampleWriteService
    {
        Task ProcessSamples(IEnumerable<SampleDto> dto);
        Task DeleteSamplesIfExists(IEnumerable<SampleIdDto> dto);
    }
}
