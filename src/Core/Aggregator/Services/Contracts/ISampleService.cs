using Biobanks.Entities.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Services.Contracts
{
    public interface ISampleService
    {
        Task<IEnumerable<LiveSample>> ListSimilarSamples(LiveSample sample);

        Task<IEnumerable<LiveSample>> ListDirtySamples();

        Task CleanSamples(IEnumerable<LiveSample> samples);

        Task DeleteFlaggedSamples();

    }
}
