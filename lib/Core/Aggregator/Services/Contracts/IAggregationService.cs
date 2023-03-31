using System.Collections.Generic;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.Api;

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
