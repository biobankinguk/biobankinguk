using System.Threading.Tasks;
using Biobanks.Aggregator.AzFunctions.Types;
using Microsoft.Azure.Functions.Worker;

namespace Biobanks.Aggregator.AzFunctions
{
    public class Function1
    {
        [Function("Function1")]
        public async Task Run([TimerTrigger("0 1 * * *", RunOnStartup = true)] TimerInfo timer)
        {
        }
    }
}
