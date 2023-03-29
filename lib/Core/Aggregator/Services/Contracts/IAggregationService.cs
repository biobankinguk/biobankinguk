using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using System.Collections.Generic;

namespace Biobanks.Aggregator.Services.Contracts
{
    public interface IAggregationService
    {
        IEnumerable<IEnumerable<LiveSample>> GroupIntoCollections(IEnumerable<LiveSample> samples);
        IEnumerable<IEnumerable<LiveSample>> GroupIntoSampleSets(IEnumerable<LiveSample> samples);
        IEnumerable<IEnumerable<LiveSample>> GroupIntoMaterialDetails(IEnumerable<LiveSample> samples);

        Collection GenerateCollection(IEnumerable<LiveSample> samples, string collectionName);
        SampleSet GenerateSampleSet(IEnumerable<LiveSample> samples);
        MaterialDetail GenerateMaterialDetail(IEnumerable<LiveSample> samples);

        string GenerateCollectionName(LiveSample sample);
    }
}
