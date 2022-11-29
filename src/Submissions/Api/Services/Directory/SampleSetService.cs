using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory;

public class SampleSetService : ISampleSetService
{
  private readonly IGenericEFRepository<SampleSet> _sampleSetRepository;

  public SampleSetService(IGenericEFRepository<SampleSet> sampleSetRepository)
  {
    _sampleSetRepository = sampleSetRepository;
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


}
