using System.Threading.Tasks;
using Biobanks.Common.Data.Entities;
using Biobanks.SubmissionJob.Dtos;

namespace Biobanks.SubmissionJob.Services.Contracts
{
    public interface ISampleValidationService
    {
        Task<StagedSample> ValidateAndPopulateSample(SampleDto dto, StagedSample sample = null);
    }
}
