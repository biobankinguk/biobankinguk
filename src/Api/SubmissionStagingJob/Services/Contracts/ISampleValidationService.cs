using System.Threading.Tasks;
using Biobanks.Common.Data.Entities;
using Biobanks.SubmissionStagingJob.Dtos;

namespace Biobanks.SubmissionStagingJob.Services.Contracts
{
    public interface ISampleValidationService
    {
        Task<StagedSample> ValidateAndPopulateSample(SampleDto dto, StagedSample sample = null);
    }
}
