using Biobanks.Submissions.Api.Services.Submissions.Contracts;
using Core.Submissions.Models;
using Core.Submissions.Services.Contracts;
using Core.Submissions.Types;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Submissions
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
        public async Task Stage(int biobankId, int submissionId, Guid blobId, string blobType, Operation op)
            => await _queueWriteService.PushAsync("operations",
                JsonSerializer.Serialize(
                    new OperationsQueueItem
                    {
                        SubmissionId = submissionId,
                        Operation = op,
                        BlobId = blobId,
                        BlobType = blobType,
                        BiobankId = biobankId
                    }
                )
            );

        /// <inheritdoc />
        public async Task Commit(int biobankId, bool replace)
            => await _queueWriteService.PushAsync("commits",
                JsonSerializer.Serialize(
                    new CommitQueueItem
                    {
                        BiobankId = biobankId,
                        Replace = replace
                    }
                )
            );

        /// <inheritdoc />
        public async Task Reject(int biobankId)
            => await _queueWriteService.PushAsync("reject",
                JsonSerializer.Serialize(
                    new RejectQueueItem
                    {
                        BiobankId = biobankId
                    }
                )
            );

    }
}
