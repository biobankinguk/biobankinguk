using Biobanks.Aggregator.Core.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core
{
    public class AggregationTask
    {
        private readonly IAggregationService _aggregationService;

        public AggregationTask(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        public async Task Run()
        {
            var dirtySamples = await _aggregationService.ListDirtySamplesAsync();

            // Group Into New/Exisiting Collections
            var collections = await _aggregationService.GroupCollections(dirtySamples);

            // Delete Samples With isDeleted Flag
            await _aggregationService.DeleteFlaggedSamplesAsync();

            // Build Collections
            foreach (var collection in collections)
            {
                var samples = await _aggregationService.ListRelevantSamplesAsync(collection);

                if (samples.Any())
                {
                    // Generate and Populate SampleSets
                    collection.SampleSets = await _aggregationService.GroupSampleSets(samples);

                    if (collection.CollectionId == default)
                    {
                        await _aggregationService.AddCollectionAsync(collection);
                    }
                    else
                    {
                        await _aggregationService.UpdateCollectionAsync(collection);
                    }
                }
                else
                {
                    await _aggregationService.DeleteCollectionAsync(collection);
                }
            }
        }
    }
}
