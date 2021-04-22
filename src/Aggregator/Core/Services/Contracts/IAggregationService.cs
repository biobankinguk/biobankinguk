using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface IAggregationService
    {
        #region Aggregation Service
        IEnumerable<SampleSet> GroupSampleSets(IEnumerable<LiveSample> samples);

        Task<IEnumerable<Collection>> GroupCollections(IEnumerable<LiveSample> samples);
        #endregion

        #region LiveSample Service
        Task<IEnumerable<LiveSample>> ListRelevantSamplesAsync(Collection collection);

        Task<IEnumerable<LiveSample>> ListDirtySamplesAsync();
        
        Task DeleteFlaggedSamplesAsync();
        #endregion

        #region Collection Service
        Task DeleteCollectionAsync(Collection collection);

        Task UpdateCollectionAsync(Collection collection);

        Task AddCollectionAsync(Collection collection);
        #endregion
    }
}
