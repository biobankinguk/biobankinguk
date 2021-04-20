using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface IAggregationService
    {
        Task<IEnumerable<Collection>> GroupByCollectionsAsync(IEnumerable<LiveSample> samples);

        Task<IEnumerable<LiveSample>> ListCollectionSamplesAsync(Collection collection);

        Task<IEnumerable<LiveSample>> ListDirtySamplesAsync();
        
        Task DeleteFlaggedSamplesAsync();

        Task DeleteCollectionAsync(Collection collection);
    }
}
