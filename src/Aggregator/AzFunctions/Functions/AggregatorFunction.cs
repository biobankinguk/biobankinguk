using System.Threading.Tasks;
using Biobanks.Aggregator.AzFunctions.Types;
using Microsoft.Azure.Functions.Worker;

namespace Biobanks.Aggregator.AzFunctions
{
    public class AggregatorFunction
    {
        [Function("Aggregator")]
        public void Run([TimerTrigger("0 1 * * *")] TimerInfo timer)
        {


        }
    }
}
