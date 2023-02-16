using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory;

public class SampleSetService : ISampleSetService
{
  private readonly IOrganisationDirectoryService _organisationService;
  private readonly IBiobankIndexService _indexService;
  private readonly ApplicationDbContext _context;
  public SampleSetService(IOrganisationDirectoryService organisationService, IBiobankIndexService indexService, ApplicationDbContext context)
  {
    _organisationService = organisationService;
    _indexService = indexService;
    _context = context;

  }
  public async Task AddSampleSetAsync(SampleSet sampleSet)
  {
    // Add new SampleSet
    _context.SampleSets.Add(sampleSet);
    await _context.SaveChangesAsync();

    // Update collection's timestamp
    var collection = await _context.Collections.FindAsync(sampleSet.CollectionId);
    collection.LastUpdated = DateTime.Now;
    await _context.SaveChangesAsync();

    // Index New SampleSet
    if (!await _organisationService.IsSuspended(collection.OrganisationId))
      await _indexService.IndexSampleSet(sampleSet.Id);
  }
  public async Task UpdateSampleSetAsync(SampleSet sampleSet)
  {
    // Update SampleSet

    var existingSampleSet = await _context.SampleSets
            .Where(x => x.Id == sampleSet.Id)
            .Include(x => x.Collection)
            .Include(x => x.MaterialDetails)
            .FirstOrDefaultAsync();

    existingSampleSet.SexId = sampleSet.SexId;
    existingSampleSet.AgeRangeId = sampleSet.AgeRangeId;
    existingSampleSet.DonorCountId = sampleSet.DonorCountId;
    existingSampleSet.Collection.LastUpdated = DateTime.Now;

    // Existing MaterialDetails
    foreach (var existingMaterialDetail in existingSampleSet.MaterialDetails.ToList())
    {
      var materialDetail = sampleSet.MaterialDetails.FirstOrDefault(x => x.Id == existingMaterialDetail.Id);

      // Update MaterialDetail 
      if (materialDetail != null)
      {
        existingMaterialDetail.MaterialTypeId = materialDetail.MaterialTypeId;
        existingMaterialDetail.StorageTemperatureId = materialDetail.StorageTemperatureId;
        existingMaterialDetail.MacroscopicAssessmentId = materialDetail.MacroscopicAssessmentId;
        existingMaterialDetail.ExtractionProcedureId = materialDetail.ExtractionProcedureId;
        existingMaterialDetail.PreservationTypeId = materialDetail.PreservationTypeId;
        existingMaterialDetail.CollectionPercentageId = materialDetail.CollectionPercentageId;
      }
      // Delete MaterialDetail
      else
      {
        _context.MaterialDetails.Remove(existingMaterialDetail);
      }
    }

    // New MaterialDetails
    foreach (var materialDetail in sampleSet.MaterialDetails.Where(x => x.Id == default))
    {
      _context.Add(
          new MaterialDetail
          {
            SampleSetId = existingSampleSet.Id,
            MaterialTypeId = materialDetail.MaterialTypeId,
            StorageTemperatureId = materialDetail.StorageTemperatureId,
            MacroscopicAssessmentId = materialDetail.MacroscopicAssessmentId,
            ExtractionProcedureId = materialDetail.ExtractionProcedureId,
            PreservationTypeId = materialDetail.PreservationTypeId,
            CollectionPercentageId = materialDetail.CollectionPercentageId
          }
      );
    }

    await _context.SaveChangesAsync();

    // Update Search Index
    if (!await _organisationService.IsSuspended(existingSampleSet.Collection.OrganisationId))
    {
      await _indexService.UpdateSampleSetDetails(sampleSet.Id);
    }
  }
  
  public async Task DeleteSampleSetAsync(int id)
  {
    //we need to check if the sampleset belongs to a suspended bb, BEFORE we delete the sampleset
    var sampleSet = await _context.SampleSets.Where(x=> x.Id == id).SingleOrDefaultAsync();
    var collection = await _context.Collections.Where(x => x.CollectionId == sampleSet.CollectionId).SingleOrDefaultAsync();
    var suspended = await _organisationService.IsSuspended(collection.OrganisationId);

    //delete materialdetails to avoid orphaned data or integrity errors
    var materialEntity = await _context.MaterialDetails
      .FirstOrDefaultAsync(x => x.SampleSetId == id);

    if(materialEntity != null)
      _context.MaterialDetails.Remove(materialEntity);

    var sampleEntity = await _context.SampleSets
      .FirstOrDefaultAsync(x => x.Id == id);

    _context.SampleSets.Remove(sampleEntity);

    await _context.SaveChangesAsync();

    if (!suspended)
      _indexService.DeleteSampleSet(id);
  }
  
  public async Task<IEnumerable<SampleSet>> GetSampleSetsByIdsForIndexingAsync(
     IEnumerable<int> sampleSetIds)
  {
    var sampleSets = await _context.SampleSets
        .AsNoTracking()
        .Where(x => sampleSetIds.Contains(x.Id) && !x.Collection.Organisation.IsSuspended)
        .Include(x => x.Collection)
        .Include(x => x.Collection.OntologyTerm)
        .Include(x => x.Collection.Organisation)
        .Include(x => x.Collection.Organisation.OrganisationNetworks.Select(on => on.Network))
        .Include(x => x.Collection.CollectionStatus)
        .Include(x => x.Collection.ConsentRestrictions)
        .Include(x => x.Collection.AccessCondition)
        .Include(x => x.Collection.CollectionType)
        .Include(x => x.Collection.AssociatedData.Select(ad => ad.AssociatedDataType))
        .Include(x => x.AgeRange)
        .Include(x => x.DonorCount)
        .Include(x => x.Sex)
        .Include(x => x.MaterialDetails)
        .Include(x => x.Collection.Organisation.OrganisationServiceOfferings.Select(s => s.ServiceOffering))
        .Include(x => x.MaterialDetails)
            .ThenInclude(y => y.CollectionPercentage)
        .Include(x => x.MaterialDetails)
            .ThenInclude(y => y.MacroscopicAssessment)
        .Include(x => x.MaterialDetails)
            .ThenInclude(y => y.MaterialType)
        .Include(x => x.MaterialDetails)
            .ThenInclude(y => y.StorageTemperature)
        .Include(x => x.Collection.Organisation.Country)
        .Include(x => x.Collection.Organisation.County)
        .FirstOrDefaultAsync();

    return (IEnumerable<SampleSet>)sampleSets;
  }
  public async Task<int> GetSampleSetCountAsync()
  => await _context.SampleSets.CountAsync();

  public async Task<IEnumerable<int>> GetAllSampleSetIdsAsync()
    => await _context.SampleSets.Select(x => x.Id).ToListAsync();

  public async Task<int> GetIndexableSampleSetCountAsync()
    => (await GetSampleSetsByIdsForIndexingAsync(await GetAllSampleSetIdsAsync())).Count();
  public async Task<int> GetSuspendedSampleSetCountAsync()
     => await _context.SampleSets.CountAsync(
         x => x.Collection.Organisation.IsSuspended);

  public async Task<SampleSet> GetSampleSetByIdAsync(int id)
      => (

      await _context.SampleSets
            .Where(x => x.Id == id)
            .Include(x => x.Sex)
            .Include(x => x.AgeRange)
            .Include(x => x.DonorCount)
            .Include(x => x.MaterialDetails)
                .ThenInclude(y => y.CollectionPercentage)
            .Include(x => x.MaterialDetails)
                .ThenInclude(y => y.MacroscopicAssessment)
            .Include(x => x.MaterialDetails)
                .ThenInclude(y => y.MaterialType)
            .Include(x => x.MaterialDetails)
                .ThenInclude(y => y.StorageTemperature)
            .Include(x => x.MaterialDetails)
                .ThenInclude(y => y.ExtractionProcedure)
            .Include(x => x.MaterialDetails)
                .ThenInclude(x => x.PreservationType)
            .FirstOrDefaultAsync()
  );
}
