using Biobanks.Data;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class DiseaseStatusService : IDiseaseStatusService
    {
    private readonly ApplicationDbContext _context;

    public DiseaseStatusService (
            ApplicationDbContext context
            )
        {
      _context = context;
    }
    public async Task<SnomedTag> GetSnomedTagByDescription(string description)
       => await _context.SnomedTags
      .AsNoTracking()
      .Where(x => x.Value == description)
      .SingleOrDefaultAsync();

  }
}
