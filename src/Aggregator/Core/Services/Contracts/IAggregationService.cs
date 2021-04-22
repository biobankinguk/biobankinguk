using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface IAggregationService
    {
        Task<IEnumerable<SampleSet>> GroupSampleSets(IEnumerable<LiveSample> samples);
        
        Task<IEnumerable<Collection>> GroupCollections(IEnumerable<LiveSample> samples);

        Task<IEnumerable<MaterialDetail>> GenerateMaterialDetails(IEnumerable<LiveSample> samples);
    }
}
