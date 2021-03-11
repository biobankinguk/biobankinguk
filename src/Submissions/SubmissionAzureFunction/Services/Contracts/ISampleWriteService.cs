using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.SubmissionAzureFunction.Dtos;

namespace Biobanks.SubmissionAzureFunction.Services.Contracts
{
    public interface ISampleWriteService
    {
        Task ProcessSamples(IEnumerable<SampleDto> dto);
        Task DeleteSamplesIfExists(IEnumerable<SampleIdDto> dto);
    }
}
