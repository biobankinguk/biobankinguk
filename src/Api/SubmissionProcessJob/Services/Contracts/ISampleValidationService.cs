using System.Threading.Tasks;
using Biobanks.Common.Data.Entities;
using Biobanks.SubmissionProcessJob.Dtos;

namespace Biobanks.SubmissionProcessJob.Services.Contracts
{
    public interface ISampleValidationService
    {
        Task<StagedSample> ValidateAndPopulateSample(SampleDto dto, StagedSample sample = null);
    }
}
