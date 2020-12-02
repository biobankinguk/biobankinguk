using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.SubmissionJob.Dtos;

namespace Biobanks.SubmissionJob.Services.Contracts
{
    public interface ISampleWriteService
    {
        Task ProcessSamples(IEnumerable<SampleDto> dto);
        Task DeleteSamplesIfExists(IEnumerable<SampleIdDto> dto);
    }
}
