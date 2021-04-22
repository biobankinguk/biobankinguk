using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface ISampleService
    {
        Task<IEnumerable<LiveSample>> ListRelevantSamplesAsync(Collection collection);
        
        Task<IEnumerable<LiveSample>> ListDirtySamplesAsync();

        Task DeleteFlaggedSamplesAsync();
    }
}
