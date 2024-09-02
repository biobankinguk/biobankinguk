using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
  public class OntologyTermService : IOntologyTermService
  {
    private readonly ApplicationDbContext _db;
    private readonly ICapabilityService _capabilityService;

    public OntologyTermService(ApplicationDbContext db, ICapabilityService capabilityService)
    {
      _db = db;
      _capabilityService = capabilityService;
    }

    protected IQueryable<OntologyTerm> ReadOnlyQuery(
      string id = null, string value = null, List<string> tags = null, bool onlyDisplayable = false,
      bool filterById = true, bool doLowerCaseMatch = false, bool doExactMatch = false)
    {
      var query = _db.OntologyTerms
        .AsNoTracking()
        .Include(x => x.SnomedTag)
        .Include(x => x.MaterialTypes)
        .Include(x => x.AssociatedDataTypes)
        .Where(x => x.DisplayOnDirectory || !onlyDisplayable);

      // Filter By ID
      if (!string.IsNullOrEmpty(id) && filterById)
        query = query.Where(x => x.Id == id);

      // Filter By OntologyTerm and OtherTerms
      if (!string.IsNullOrEmpty(value))
        query =
          doLowerCaseMatch
            ? query.Where(x =>
              x.Value.ToLower().Contains(value.ToLower()) ||
              x.OtherTerms.ToLower().Contains(value.ToLower()))
            : query.Where(x =>
              x.Value.Contains(value) ||
              x.OtherTerms.Contains(value));
      
      if (!string.IsNullOrEmpty(value) && doExactMatch)
        query = query.Where(x => x.Value == value);
      
      // Filter By SnomedTag
      if (tags != null)
        query = query.Where(x =>
          tags.Any()
            ? x.SnomedTag != null && tags.Contains(x.SnomedTag.Value) // Term With Included Tag
            : x.SnomedTag == null); // Terms Without Tags

      return query;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OntologyTerm>> List(
      string value = null, List<string> tags = null, bool onlyDisplayable = false, bool doLowerCaseMatch = false)
      => await ReadOnlyQuery(id: null, value, tags, onlyDisplayable, doLowerCaseMatch: doLowerCaseMatch).ToListAsync();


    /// <inheritdoc/>
    public async Task<IEnumerable<OntologyTerm>> ListPaginated(
      int skip, int take, string value = null, List<string> tags = null, bool onlyDisplayable = false)
    {
      return await ReadOnlyQuery(id: null, value, tags, onlyDisplayable)
        .OrderByDescending(x => x.DisplayOnDirectory).ThenBy(x => x.Value)
        .Skip(skip)
        .Take(take)
        .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<OntologyTerm> Get(string id = null, string value = null, List<string> tags = null,
      bool onlyDisplayable = false, bool doExactMatch = false)
      => await ReadOnlyQuery(id, value, tags, onlyDisplayable, doExactMatch: doExactMatch).FirstOrDefaultAsync();

    /// <inheritdoc/>
    public async Task<int> Count(string value = null, List<string> tags = null)
      => await ReadOnlyQuery(id: null, value, tags).CountAsync();

    /// <inheritdoc/>
    public async Task<int> CountCollectionCapabilityUsage(string ontologyTermId)
      => await _db.Collections.CountAsync(x => x.OntologyTermId == ontologyTermId)
         + await _db.DiagnosisCapabilities.CountAsync(x => x.OntologyTermId == ontologyTermId);

    /// <inheritdoc/>
    public async Task<bool> Exists(string id = null, string value = null, List<string> tags = null,
      bool onlyDisplayable = false, bool filterById = true)
      => await ReadOnlyQuery(id, value, tags, onlyDisplayable, filterById).AnyAsync(x => x.Id != id);

    /// <inheritdoc/>
    public async Task<bool> IsInUse(string id)
      => (await CountCollectionCapabilityUsage(id) > 0);

    /// <inheritdoc/>
    public async Task<OntologyTerm> Create(OntologyTerm ontologyTerm)
    {
      // Get Attached References To MaterialType
      var materialTypeIds = ontologyTerm?.MaterialTypes?.Select(x => x.Id) ?? new List<int>();

      var materialTypes = await _db.MaterialTypes
        .Where(x => materialTypeIds.Contains(x.Id))
        .ToListAsync();

      var newTerm = new OntologyTerm
      {
        Id = ontologyTerm.Id,
        Value = ontologyTerm.Value,
        OtherTerms = ontologyTerm.OtherTerms,
        SnomedTagId = ontologyTerm.SnomedTagId,
        DisplayOnDirectory = ontologyTerm.DisplayOnDirectory,
        MaterialTypes = materialTypes,
        AssociatedDataTypes = ontologyTerm.AssociatedDataTypes
      };

      _db.OntologyTerms.Add(newTerm);

      await _db.SaveChangesAsync();

      // Return OntologyTerm With Idenity ID
      return newTerm;
    }

    /// <inheritdoc/>
    public async Task<OntologyTerm> Update(OntologyTerm ontologyTerm)
    {
      // Reference Updated MaterialTypes By Id
      var materialIds = ontologyTerm.MaterialTypes?.Select(x => x.Id) ?? Enumerable.Empty<int>();

      // Update Current Term
      var currentTerm = await _db.OntologyTerms
        .Include(x => x.MaterialTypes)
        .Include(x => x.AssociatedDataTypes)
        .FirstAsync(x => x.Id == ontologyTerm.Id);

      currentTerm.Value = ontologyTerm.Value;
      currentTerm.OtherTerms = ontologyTerm.OtherTerms;
      currentTerm.DisplayOnDirectory = ontologyTerm.DisplayOnDirectory;
      currentTerm.SnomedTag = ontologyTerm.SnomedTag;
      currentTerm.AssociatedDataTypes = ontologyTerm.AssociatedDataTypes;

      // Link To Existing Material Types
      currentTerm.MaterialTypes = await _db.MaterialTypes.Where(x => materialIds.Contains(x.Id)).ToListAsync();

      await _db.SaveChangesAsync();

      await _capabilityService.UpdateCapabilitiesOntologyOtherTerms(ontologyTerm.Value);

      return currentTerm;
    }

    /// <inheritdoc/>
    public async Task Delete(string id)
    {
      var ontologyTerm = new OntologyTerm { Id = id };

      _db.OntologyTerms.Attach(ontologyTerm);
      _db.OntologyTerms.Remove(ontologyTerm);

      await _db.SaveChangesAsync();
    }

    public async Task<List<OntologyTerm>> GetByAssociatedDataType(int id)
    {
      var list = await _db.AssociatedDataTypes
        .Where(p => p.Id == id)
        .SelectMany(p => p.OntologyTerms)
        .OrderByDescending(p => p.Id)
        .ToListAsync();
      return list;
    }

    public async Task<List<AssociatedDataType>> ListAssociatedDataTypesByOntologyTerm(string id)
    {
      var list = await _db.OntologyTerms
        .Where(p => p.Id == id)
        .SelectMany(p => p.AssociatedDataTypes)
        .OrderByDescending(p => p.Id)
        .ToListAsync();

      return list;
    }

    public async Task<List<OntologyTerm>> GetOntologyTermsFromList(List<string> input)
    {
      var list = await _db.OntologyTerms
        .Where(r => input.Contains(r.Id))
        .ToListAsync();
      return list;
    }

    public async Task<List<AssociatedDataType>> GetAssociatedDataFromList(List<int> input)
    {
      var list = await _db.AssociatedDataTypes
        .Where(r => input.Contains(r.Id))
        .ToListAsync();
      return list;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SnomedTag>> ListSnomedTags()
      => await _db.SnomedTags.ToListAsync();

    /// <inheritdoc/>
    public async Task<SnomedTag> GetSnomedTagByDescription(string description)
      => await _db.SnomedTags.Where(x => x.Value == description).SingleOrDefaultAsync();
  }
}
