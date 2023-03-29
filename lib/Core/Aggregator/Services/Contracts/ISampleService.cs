using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data.Entities.Api;

namespace Biobanks.Aggregator.Services.Contracts
{
    public interface ISampleService
    {
        Task<IEnumerable<LiveSample>> ListSimilarSamples(IEnumerable<LiveSample> samples);

        Task<IEnumerable<LiveSample>> ListDirtyExtractedSamples();

        Task<IEnumerable<LiveSample>> ListDirtyNonExtractedSamples();

        Task CleanSamples(IEnumerable<LiveSample> samples);

        Task DeleteFlaggedSamples();

    }
}
