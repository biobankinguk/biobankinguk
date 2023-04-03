using System.Linq;
using Biobanks.Data;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
  public class BiobankAuthorizationService : IBiobankAuthorizationService
  {
    private readonly ApplicationDbContext _db;

    public BiobankAuthorizationService(ApplicationDbContext db)
    {
      _db = db;
    }
    
    public bool CanThisBiobankAdministerThisCollection(int biobankId, int collectionId)
      => _db.Collections.AsNoTracking()
        .Any(x => x.OrganisationId == biobankId &&
                  x.CollectionId == collectionId);

    public bool CanThisBiobankAdministerThisSampleSet(int biobankId, int sampleSetId)
      => _db.SampleSets.AsNoTracking()
        .Any(x => x.Collection.OrganisationId == biobankId &&
                  x.Id == sampleSetId);

    public bool CanThisBiobankAdministerThisCapability(int biobankId, int capabilityId)
      => _db.DiagnosisCapabilities.AsNoTracking()
        .Any(x => x.OrganisationId == biobankId &&
                  x.DiagnosisCapabilityId == capabilityId);
  }
}
