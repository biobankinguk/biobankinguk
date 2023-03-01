using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Search.Dto.Documents;
using Biobanks.Search.Dto.PartialDocuments;
using Biobanks.Search.Legacy;
using Biobanks.Submissions.Api.Models.Directory;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.Directory.Extensions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Biobanks.Submissions.Api.Services.Directory;

public class CapabilityService : ICapabilityService
{

    private readonly ApplicationDbContext _db;
    private readonly IIndexProvider _indexProvider;
    private readonly IReferenceDataCrudService<DonorCount> _donorCountService;
    private readonly IOrganisationDirectoryService _organisationService;
    private readonly IBiobankIndexService _indexService;

  public CapabilityService(
        ApplicationDbContext db,
        IIndexProvider indexProvider,
        IReferenceDataCrudService<DonorCount> donorCountService,
        IOrganisationDirectoryService organisationService,
        IBiobankIndexService indexService
        )
    {
        _db = db;
        _indexProvider = indexProvider;
        _donorCountService = donorCountService;
        _organisationService = organisationService;
        _indexService = indexService;
  }

    protected IQueryable<OntologyTerm> ReadOnlyQuery(
        string id = null, string value = null, List<string> tags = null, bool onlyDisplayable = false, bool filterById = true)
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
        query = query.Where(x => x.Value.Contains(value) || x.OtherTerms.Contains(value));

      // Filter By SnomedTag
      if (tags != null)
        query = query.Where(x =>
            tags.Any()
                ? x.SnomedTag != null && tags.Contains(x.SnomedTag.Value) // Term With Included Tag
                : x.SnomedTag == null); // Terms Without Tags

      return query;
    }
    private async Task<OntologyTerm> Get(string id = null, string value = null, List<string> tags = null, bool onlyDisplayable = false)
      => await ReadOnlyQuery(id, value, tags, onlyDisplayable).FirstOrDefaultAsync();
    
    /// <inheritdoc/>
    public async Task<IEnumerable<int>> GetAllCapabilityIdsAsync()
          => (await _db.DiagnosisCapabilities.Select(x => x.DiagnosisCapabilityId)
                  .ToListAsync()
              );
    
    /// <inheritdoc/>
    public async Task<int> GetCapabilityCountAsync()
        => await _db.DiagnosisCapabilities.CountAsync();
    
    /// <inheritdoc/>
    public async Task<int> GetIndexableCapabilityCountAsync()
        => (await _indexService.GetCapabilitiesByIdsForIndexingAsync(await GetAllCapabilityIdsAsync())).Count();
    
    /// <inheritdoc/>
    public async Task<int> GetSuspendedCapabilityCountAsync()
        => await _db.DiagnosisCapabilities
            .AsNoTracking()
            .Where(x => x.Organisation.IsSuspended)
            .CountAsync();

    
    /// <inheritdoc/>
    public async Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexDeletionAsync(
        IEnumerable<int> capabilityIds)
        => (await _db.DiagnosisCapabilities
                .AsNoTracking()
                .Where(x => capabilityIds.Contains(x.DiagnosisCapabilityId))
                .Include(x => x.Organisation)
                .Include(x => x.Organisation.OrganisationNetworks)
                    .ThenInclude(on => on.Network)
                .Include(x => x.Organisation.OrganisationServiceOfferings)
                    .ThenInclude(s => s.ServiceOffering)
                .Include(x => x.OntologyTerm)
                .Include(x => x.AssociatedData)
                .Include(x => x.SampleCollectionMode)
                .ToListAsync()
            );
    
    /// <inheritdoc/>
    public async Task<DiagnosisCapability> GetCapabilityByIdForIndexingAsync(int id)
        => (await _db.DiagnosisCapabilities
                .AsNoTracking()
                .Where(x => x.DiagnosisCapabilityId == id)
                .Include(x => x.Organisation)
                .Include(x => x.Organisation.OrganisationNetworks)
                  .ThenInclude(on => @on.Network)
                .Include(x => x.Organisation.OrganisationServiceOfferings)
                  .ThenInclude(x => x.ServiceOffering)
                .Include(x => x.OntologyTerm)
                .Include(x => x.AssociatedData)
                .Include(x => x.AssociatedData)
                  .ThenInclude(x => x.AssociatedDataType)
                .Include(x => x.AssociatedData)
                  .ThenInclude(x => x.AssociatedDataProcurementTimeframe)
                .Include(x => x.SampleCollectionMode)
                .FirstOrDefaultAsync()
            );

    /// <inheritdoc/>
    public async Task<IEnumerable<int>> GetCapabilityIdsByOntologyTermAsync(string ontologyTerm)
        => (await _db.DiagnosisCapabilities
                .AsNoTracking()
                .Where(x => x.OntologyTerm.Value == ontologyTerm)
                .Select(x => x.DiagnosisCapabilityId)
                .ToListAsync()
            );
    
    /// <inheritdoc/>
    public async Task<DiagnosisCapability> GetCapabilityByIdAsync(int id)
        => (await _db.DiagnosisCapabilities
                .AsNoTracking()
                .Where(x => x.DiagnosisCapabilityId == id)
                .Include(x => x.OntologyTerm)
                .Include(x => x.AssociatedData)
                  .ThenInclude(x => x.AssociatedDataType)
                .Include(x => x.AssociatedData)
                  .ThenInclude(x => x.AssociatedDataProcurementTimeframe)
                .Include(x => x.SampleCollectionMode)
                .FirstOrDefaultAsync()
            );

    /// <inheritdoc/>
    public async Task<IEnumerable<DiagnosisCapability>> ListCapabilitiesAsync(int organisationId)
        => (await _db.DiagnosisCapabilities
                .AsNoTracking()
                .Where(x => x.OrganisationId == organisationId)
                .Include(x => x.OntologyTerm)
                .Include(x => x.SampleCollectionMode)
                .ToListAsync()
            );
    
    /// <inheritdoc/>
    public async Task AddCapabilityAsync(CapabilityDTO capabilityDTO, IEnumerable<CapabilityAssociatedData> associatedData)
      {
        var ontologyTerm = await Get(value: capabilityDTO.OntologyTerm, onlyDisplayable: true);

        var capability = new DiagnosisCapability
        {
          OrganisationId = capabilityDTO.OrganisationId,
          OntologyTermId = ontologyTerm.Id,
          AnnualDonorExpectation = capabilityDTO.AnnualDonorExpectation.Value,
          AssociatedData = associatedData.ToList(),
          SampleCollectionModeId = capabilityDTO.SampleCollectionModeId,
          LastUpdated = DateTime.Now
        };

        _db.DiagnosisCapabilities.Add(capability);

        await _db.SaveChangesAsync();

        if (!await _organisationService.IsSuspended(capability.OrganisationId))
          await _indexService.IndexCapability(capability.DiagnosisCapabilityId);
      }

      /// <inheritdoc/>
      public async Task UpdateCapabilityAsync(CapabilityDTO capabilityDTO, IEnumerable<CapabilityAssociatedData> associatedData)
        {
        var existingCapability = (await _db.DiagnosisCapabilities
          .AsNoTracking()
          .Where(x => x.DiagnosisCapabilityId == capabilityDTO.Id)
          .Include(x => x.AssociatedData)
          .FirstOrDefaultAsync());

          existingCapability.AssociatedData.Clear();

          var ontologyTerm = await Get(value: capabilityDTO.OntologyTerm, onlyDisplayable: true);

          existingCapability.OntologyTermId = ontologyTerm.Id;
          existingCapability.AnnualDonorExpectation = capabilityDTO.AnnualDonorExpectation.Value;
          existingCapability.SampleCollectionModeId = capabilityDTO.SampleCollectionModeId;
          existingCapability.LastUpdated = DateTime.Now;

          existingCapability.AssociatedData = associatedData.ToList();

          await _db.SaveChangesAsync();

          if (!await _organisationService.IsSuspended(existingCapability.OrganisationId))
            await UpdateCapabilityDetails(existingCapability.DiagnosisCapabilityId);
        }

      /// <inheritdoc/>
      public async Task DeleteCapabilityAsync(int id)
      {
            var capability = await _db.DiagnosisCapabilities.FindAsync(id);

            var entity = await _db.DiagnosisCapabilities
              .Where(x => x.DiagnosisCapabilityId == id)
              .FirstOrDefaultAsync();
            
            _db.DiagnosisCapabilities.Remove(entity);

            if (!await _organisationService.IsSuspended(capability.OrganisationId))
              _indexService.DeleteCapability(id);
            await _db.SaveChangesAsync();
      }

      /// <inheritdoc/>
      public async Task UpdateCapabilityDetails(int capabilityId)
      {
          // Get the entire capability object from the database.
          var updatedCapability = await GetCapabilityByIdForIndexingAsync(capabilityId);

          // Get the donor counts and get expectations from them
          var donorExpectation = DiagnosisCapabilityExtensions.GetAnnualDonorExpectationRange(
                      await _donorCountService.List(),
                      updatedCapability.AnnualDonorExpectation);

          //Prep metadata for the facet value
          var donorExpectationMetadata = JsonConvert.SerializeObject(new
          {
              Name = donorExpectation.Key,
              SortOrder = donorExpectation.Value
          });

          // Queue up a job to update the capability in the search index.
          BackgroundJob.Enqueue(() => _indexProvider.UpdateCapabilitySearchDocument(
              updatedCapability.DiagnosisCapabilityId,
              new PartialCapability
              {
                  OntologyTerm = updatedCapability.OntologyTerm.Value,
                  Protocols = updatedCapability.SampleCollectionMode.Value,
                  AnnualDonorExpectation = donorExpectation.Key,
                  AnnualDonorExpectationMetadata = donorExpectationMetadata,
                  AssociatedData = updatedCapability.AssociatedData.Select(ad => new AssociatedDataDocument
                  {
                      Text = ad.AssociatedDataType.Value,
                      Timeframe = ad.AssociatedDataProcurementTimeframe.Value,
                      TimeframeMetadata = JsonConvert.SerializeObject(new
                      {
                          Name = ad.AssociatedDataProcurementTimeframe.Value,
                          ad.AssociatedDataProcurementTimeframe.SortOrder
                      })
                  }),
                  OntologyOtherTerms = SampleSetExtensions.ParseOtherTerms(updatedCapability.OntologyTerm.OtherTerms)
              }));
      }

  public async Task UpdateCapabilitiesOntologyOtherTerms(string ontologyTerm)
  {
    var capabilityIds = await _db.DiagnosisCapabilities
      .AsNoTracking()
      .Where(x => x.OntologyTerm.Value == ontologyTerm)
      .Select(x => x.DiagnosisCapabilityId)
      .ToListAsync();

    // Update all search documents that are relevant to this collection.
    foreach (var capabilityId in capabilityIds)
    {
      await UpdateCapabilityDetails(capabilityId);
    }
  }
}
