using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface IAggregationService
    {
        IEnumerable<IEnumerable<LiveSample>> GroupIntoCollections(IEnumerable<LiveSample> samples);
        IEnumerable<IEnumerable<LiveSample>> GroupIntoSampleSets(IEnumerable<LiveSample> samples);
        IEnumerable<IEnumerable<LiveSample>> GroupIntoMaterialDetails(IEnumerable<LiveSample> samples);

        Task<Collection> GenerateCollection(IEnumerable<LiveSample> samples);
        Task<SampleSet> GenerateSampleSet(IEnumerable<LiveSample> samples);
        Task<MaterialDetail> GenerateMaterialDetail(IEnumerable<LiveSample> samples);
    }
}
