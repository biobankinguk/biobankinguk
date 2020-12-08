using System.Threading.Tasks;
using Biobanks.Common.Data.Entities;
using Biobanks.SubmissionAzureFunction.Dtos;

namespace Biobanks.SubmissionAzureFunction.Services.Contracts
{
    public interface ISampleValidationService
    {
        Task<StagedSample> ValidateAndPopulateSample(SampleDto dto, StagedSample sample = null);
    }
}
