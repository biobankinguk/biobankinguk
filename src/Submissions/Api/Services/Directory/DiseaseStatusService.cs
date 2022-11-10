using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class DiseaseStatusService : IDiseaseStatusService
    {
        private readonly IGenericEFRepository<SnomedTag> _snomedTagRepository;

        public DiseaseStatusService (
            IGenericEFRepository<SnomedTag> snomedTagRepository
            )
        {
            _snomedTagRepository = snomedTagRepository;
        }
        public async Task<SnomedTag> GetSnomedTagByDescription(string description)
            => (await _snomedTagRepository.ListAsync(filter: x => x.Value == description)).SingleOrDefault();
    }
}
