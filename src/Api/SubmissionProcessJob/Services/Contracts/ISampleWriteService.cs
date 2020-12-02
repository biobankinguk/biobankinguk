using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.SubmissionProcessJob.Dtos;

namespace Biobanks.SubmissionProcessJob.Services.Contracts
{
    public interface ISampleWriteService
    {
        Task ProcessSamples(IEnumerable<SampleDto> dto);
        Task DeleteSamplesIfExists(IEnumerable<SampleIdDto> dto);
    }
}
