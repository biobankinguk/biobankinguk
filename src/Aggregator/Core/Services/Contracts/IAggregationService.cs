using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface IAggregationService
    {

        Task<IEnumerable<Collection>> GroupSamples(IEnumerable<LiveSample> samples);

        Task<IEnumerable<LiveSample>> ListRelevantSamplesAsync(Collection collection);

        Task<IEnumerable<LiveSample>> ListDirtySamplesAsync();
        
        Task DeleteFlaggedSamplesAsync();

        Task DeleteCollectionAsync(Collection collection);

        Task UpdateCollectionAsync(Collection collection);

        Task AddCollectionAsync(Collection collection);
    }
}
