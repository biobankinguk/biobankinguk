using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class DiseaseStatusService : IDiseaseStatusService
    {
    private readonly ApplicationDbContext _db;

    public DiseaseStatusService (
            ApplicationDbContext db
            )
        {
      _db = db;
    }
    public async Task<SnomedTag> GetSnomedTagByDescription(string description)
       => await _db.SnomedTags
      .AsNoTracking()
      .Where(x => x.Value == description)
      .SingleOrDefaultAsync();

  }
}
