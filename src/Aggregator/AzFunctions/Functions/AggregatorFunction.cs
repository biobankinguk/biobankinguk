using Biobanks.Aggregator.AzFunctions.Types;
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
<<<<<<< HEAD
        public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo timer)
=======
        public async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo timer)
>>>>>>> main
            =>  await _aggregationTask.Run();
    }
}
