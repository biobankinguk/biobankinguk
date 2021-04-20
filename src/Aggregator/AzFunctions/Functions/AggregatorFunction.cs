using Biobanks.Aggregator.AzFunctions.Types;
using Biobanks.Aggregator.Core.Services.Contracts;
using Microsoft.Azure.Functions.Worker;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.AzFunctions
{
    public class AggregatorFunction
    {
        private readonly IAggregationService _aggregationService;

        public AggregatorFunction(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        [Function("Aggregator")]
        public async Task Run([TimerTrigger("*/5 * * * * *")] TimerInfo timer)
        {
            var dirtySamples = await _aggregationService.ListDirtySamples();

            if (!dirtySamples.Any())
            {
                return;
            }
        }
    }
}
