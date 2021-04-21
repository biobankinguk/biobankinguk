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
            // Find All Samples Flagged As Changed
            var dirtySamples = await _aggregationService.ListDirtySamplesAsync();

            // Group Into New/Exisiting Collections
            var collections = await _aggregationService.GroupSamples(dirtySamples);

            // Delete Samples With isDeleted Flag
            await _aggregationService.DeleteFlaggedSamplesAsync();

            // Build Collections
            foreach (var collection in collections)
            {
                var samples = await _aggregationService.ListRelevantSamplesAsync(collection);

                if (samples.Any())
                {
                    // TODO: Generate and Populate SampleSets

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
