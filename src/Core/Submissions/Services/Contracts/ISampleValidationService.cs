using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Core.Submissions.Dto;

namespace Core.Submissions.Services.Contracts
{
    public interface ISampleValidationService
    {
        Task<StagedSample> ValidateAndPopulateSample(SampleDto dto, StagedSample sample = null);
    }
}
