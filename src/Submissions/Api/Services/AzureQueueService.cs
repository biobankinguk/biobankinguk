using Biobanks.Submissions.Api.Services.Contracts;
using Biobanks.Submissions.Core.Models;
using Biobanks.Submissions.Core.Services.Contracts;
using System.Text.Json;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services
{
    /// <inheritdoc />
    public class AzureQueueService : IBackgroundJobEnqueueingService
    {
        private readonly IQueueWriteService _queueWriteService;

        /// <inheritdoc />
        public AzureQueueService(IQueueWriteService queueWriteService)
        {
            _queueWriteService = queueWriteService;
        }

        /// <inheritdoc />
        public async Task Commit(int biobankId, bool replace)
        {
            await _queueWriteService.PushAsync("commits",
                    JsonSerializer.Serialize(
                        new CommitQueueItem
                        {
                            BiobankId = biobankId,
                            Replace = replace
                        }
                        )
                    );
        }
    }
}
