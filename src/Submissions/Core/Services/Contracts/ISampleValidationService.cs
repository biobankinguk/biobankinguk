using System.Threading.Tasks;

using Biobanks.Entities.Api;
using Biobanks.Submissions.Core.Dto;

namespace Biobanks.Submissions.Core.Services.Contracts
{
    public interface ISampleValidationService
    {
        Task<StagedSample> ValidateAndPopulateSample(SampleDto dto, StagedSample sample = null);
    }
}
