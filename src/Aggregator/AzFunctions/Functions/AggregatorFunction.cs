using Biobanks.Aggregator.Core;
using Microsoft.Azure.Functions.Worker;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.AzFunctions
{
    public class AggregatorFunction
    {
        private readonly AggregationTask _aggregationTask;

        public AggregatorFunction(AggregationTask aggregationTask)
        {
            _aggregationTask = aggregationTask;
        }

        [Function("Aggregator")]
        public async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo timer)
            =>  await _aggregationTask.Run();
    }
}
