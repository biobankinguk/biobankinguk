using Biobanks.Entities.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface ISampleService
    {
        Task<IEnumerable<LiveSample>> ListSimilarSamplesAsync(LiveSample sample);

        Task<IEnumerable<LiveSample>> ListDirtySamplesAsync();

        Task CleanSamplesAsync(IEnumerable<LiveSample> samples);

        Task DeleteFlaggedSamplesAsync();

    }
}
