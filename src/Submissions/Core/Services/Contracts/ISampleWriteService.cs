using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Submissions.Core.Dto;

namespace Biobanks.Submissions.Core.Services.Contracts
{
    public interface ISampleWriteService
    {
        Task ProcessSamples(IEnumerable<SampleDto> dto);
        Task DeleteSamplesIfExists(IEnumerable<SampleIdDto> dto);
    }
}
