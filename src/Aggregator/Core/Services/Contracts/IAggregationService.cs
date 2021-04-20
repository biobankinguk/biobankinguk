using Biobanks.Entities.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface IAggregationService
    {
        Task<IEnumerable<LiveSample>> ListDirtySamplesAsync();

        Task DeleteFlaggedSamplesAsync();
    }
}
