using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory;

public class SampleSetService : ISampleSetService
{
  private readonly IGenericEFRepository<SampleSet> _sampleSetRepository;
  private readonly IGenericEFRepository<Collection> _collectionRepository;
  private readonly IOrganisationDirectoryService _organisationService;
  private readonly IBiobankIndexService _indexService;
  private readonly IGenericEFRepository<MaterialDetail> _materialDetailRepository;

  public SampleSetService(IGenericEFRepository<SampleSet> sampleSetRepository, IGenericEFRepository<Collection> collectionRepository, IOrganisationDirectoryService organisationService, IBiobankIndexService indexService, IGenericEFRepository<MaterialDetail> materialDetailRepository)
  {
    _sampleSetRepository = sampleSetRepository;
    _collectionRepository = collectionRepository;
    _organisationService = organisationService;
    _indexService = indexService;
    _materialDetailRepository = materialDetailRepository;
  }
  public async Task AddSampleSetAsync(SampleSet sampleSet)
  {
    // Add new SampleSet
    _sampleSetRepository.Insert(sampleSet);
    await _sampleSetRepository.SaveChangesAsync();

    // Update collection's timestamp
    var collection = await _collectionRepository.GetByIdAsync(sampleSet.CollectionId);
    collection.LastUpdated = DateTime.Now;
    await _collectionRepository.SaveChangesAsync();

    // Index New SampleSet
    if (!await _organisationService.IsSuspended(collection.OrganisationId))
      await _indexService.IndexSampleSet(sampleSet.Id);
  }
  public async Task UpdateSampleSetAsync(SampleSet sampleSet)
  {
    // Update SampleSet
    var existingSampleSet = (await _sampleSetRepository.ListAsync(
            tracking: true,
            filter: x => x.Id == sampleSet.Id,
            orderBy: null,
            x => x.Collection,
            x => x.MaterialDetails)
        )
        .First();

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
        _materialDetailRepository.Delete(existingMaterialDetail);
      }
    }

    // New MaterialDetails
    foreach (var materialDetail in sampleSet.MaterialDetails.Where(x => x.Id == default))
    {
      _materialDetailRepository.Insert(
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

    await _sampleSetRepository.SaveChangesAsync();
    await _materialDetailRepository.SaveChangesAsync();

    // Update Search Index
    if (!await _organisationService.IsSuspended(existingSampleSet.Collection.OrganisationId))
    {
      await _indexService.UpdateSampleSetDetails(sampleSet.Id);
    }
  }
  public async Task DeleteSampleSetAsync(int id)
  {
    //we need to check if the sampleset belongs to a suspended bb, BEFORE we delete the sampleset
    var sampleSet = await _sampleSetRepository.GetByIdAsync(id);
    var collection = await _collectionRepository.GetByIdAsync(sampleSet.CollectionId);
    var suspended = await _organisationService.IsSuspended(collection.OrganisationId);

    //delete materialdetails to avoid orphaned data or integrity errors
    await _materialDetailRepository.DeleteWhereAsync(x => x.SampleSetId == id);
    await _materialDetailRepository.SaveChangesAsync();

    await _sampleSetRepository.DeleteWhereAsync(x => x.Id == id);
    await _sampleSetRepository.SaveChangesAsync();

    if (!suspended)
      _indexService.DeleteSampleSet(id);
  }
  public async Task<IEnumerable<SampleSet>> GetSampleSetsByIdsForIndexingAsync(
     IEnumerable<int> sampleSetIds)
  {
    var sampleSets = await _sampleSetRepository.ListAsync(false,
        x => sampleSetIds.Contains(x.Id) && !x.Collection.Organisation.IsSuspended,
        null,
        x => x.Collection,
        x => x.Collection.OntologyTerm,
        x => x.Collection.Organisation,
        x => x.Collection.Organisation.OrganisationNetworks.Select(on => on.Network),
        x => x.Collection.CollectionStatus,
        x => x.Collection.ConsentRestrictions,
        x => x.Collection.AccessCondition,
        x => x.Collection.CollectionType,
        x => x.Collection.AssociatedData.Select(ad => ad.AssociatedDataType),
        x => x.AgeRange,
        x => x.DonorCount,
        x => x.Sex,
        x => x.MaterialDetails,
        x => x.Collection.Organisation.OrganisationServiceOfferings.Select(s => s.ServiceOffering),
        x => x.MaterialDetails.Select(y => y.CollectionPercentage),
        x => x.MaterialDetails.Select(y => y.MacroscopicAssessment),
        x => x.MaterialDetails.Select(y => y.MaterialType),
        x => x.MaterialDetails.Select(y => y.StorageTemperature),
        x => x.Collection.Organisation.Country,
        x => x.Collection.Organisation.County
    );

    return sampleSets;
  }
  public async Task<int> GetSampleSetCountAsync()
  => await _sampleSetRepository.CountAsync();

  public async Task<IEnumerable<int>> GetAllSampleSetIdsAsync()
    => (await _sampleSetRepository.ListAsync()).Select(x => x.Id);

  public async Task<int> GetIndexableSampleSetCountAsync()
    => (await GetSampleSetsByIdsForIndexingAsync(await GetAllSampleSetIdsAsync())).Count();
  public async Task<int> GetSuspendedSampleSetCountAsync()
     => await _sampleSetRepository.CountAsync(
         x => x.Collection.Organisation.IsSuspended);

  public async Task<SampleSet> GetSampleSetByIdAsync(int id)
      => (await _sampleSetRepository.ListAsync(false, x => x.Id == id, null,
          x => x.Sex,
          x => x.AgeRange,
          x => x.DonorCount,
          x => x.MaterialDetails,
          x => x.MaterialDetails.Select(y => y.CollectionPercentage),
          x => x.MaterialDetails.Select(y => y.MacroscopicAssessment),
          x => x.MaterialDetails.Select(y => y.MaterialType),
          x => x.MaterialDetails.Select(y => y.StorageTemperature),
          x => x.MaterialDetails.Select(y => y.ExtractionProcedure)
      )).FirstOrDefault();


}
