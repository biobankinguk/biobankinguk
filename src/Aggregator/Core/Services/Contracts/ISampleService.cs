using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface ISampleService
    {
        Task<IEnumerable<LiveSample>> ListSimilarSamplesAsync(LiveSample sample);

        Task<IEnumerable<LiveSample>> ListDirtySamplesAsync();

        Task DeleteFlaggedSamplesAsync();
    }
}
