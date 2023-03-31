using System.Threading.Tasks;
using Biobanks.Data.Entities.Api;
using Biobanks.Submissions.Dto;

namespace Biobanks.Submissions.Services.Contracts
{
    public interface ISampleValidationService
    {
        Task<StagedSample> ValidateAndPopulateSample(SampleDto dto, StagedSample sample = null);
    }
}
