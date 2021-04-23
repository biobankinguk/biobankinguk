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

        Task<Collection> GenerateCollection(IEnumerable<LiveSample> samples);


        // OLD
        Task<IEnumerable<SampleSet>> GroupSampleSets(IEnumerable<LiveSample> samples);

        Task<IEnumerable<MaterialDetail>> GroupMaterialDetails(IEnumerable<LiveSample> samples);
    }
}
