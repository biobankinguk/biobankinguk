using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Search.Dto.Documents;
using Biobanks.Search.Dto.PartialDocuments;
using Biobanks.Search.Legacy;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.Directory.Extensions;
using Hangfire;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Config;
using Microsoft.Extensions.Options;

namespace Biobanks.Submissions.Api.Services.Directory
{
  public class BiobankIndexService : IBiobankIndexService
  {
    private const int BulkIndexChunkSize = 100;

    private readonly IReferenceDataCrudService<DonorCount> _donorCountService;

    private readonly IIndexProvider _indexProvider;
    private readonly ISearchProvider _searchProvider;

    private readonly IHostEnvironment _hostEnvironment;

    private readonly TelemetryClient _telemetryClient;
    private readonly ApplicationDbContext _context;
    private readonly ElasticsearchConfig _elasticsearchConfig;

    public BiobankIndexService(
            IReferenceDataCrudService<DonorCount> donorCountService,
            IIndexProvider indexProvider,
            ISearchProvider searchProvider,
            IHostEnvironment hostEnvironment,
            TelemetryClient telemetryClient,
            ApplicationDbContext context,
            IOptions<ElasticsearchConfig> elasticsearchConfig
            )
    {
      _donorCountService = donorCountService;
      _indexProvider = indexProvider;
      _searchProvider = searchProvider;
      _hostEnvironment = hostEnvironment;
      _telemetryClient = telemetryClient;
      _context = context;
      _elasticsearchConfig = elasticsearchConfig.Value;
    }

    private async Task<IEnumerable<SampleSet>> GetSampleSetsByIdsForIndexingAsync(
       IEnumerable<int> sampleSetIds) =>
    (
      await _context.SampleSets
      .AsNoTracking()
      .Where(x => sampleSetIds.Contains(x.Id) && !x.Collection.Organisation.IsSuspended)
      .Include(x => x.Collection)
      .Include(x => x.Collection.OntologyTerm)
      .Include(x => x.Collection.Organisation)
      .Include(x => x.Collection.Organisation.OrganisationNetworks.Select(on => @on.Network))
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
      .Include(x => x.MaterialDetails.Select(y => y.CollectionPercentage))
      .Include(x => x.MaterialDetails.Select(y => y.MacroscopicAssessment))
      .Include(x => x.MaterialDetails.Select(y => y.MaterialType))
      .Include(x => x.MaterialDetails.Select(y => y.StorageTemperature))
      .Include(x => x.Collection.Organisation.Country)
      .Include(x => x.Collection.Organisation.County)
      .ToListAsync()
     );

    private async Task<SampleSet> GetSampleSetByIdForIndexingAsync(int id) => (
      await _context.SampleSets
      .AsNoTracking()
      .Where(x => x.Id == id)
      .Include(x => x.Collection)
      .Include(x => x.Collection.OntologyTerm)
      .Include(x => x.Collection.Organisation)
      .Include(x => x.Collection.Organisation.OrganisationNetworks)
          .ThenInclude(on => on.Network)
      .Include(x => x.Collection.CollectionStatus)
      .Include(x => x.Collection.ConsentRestrictions)
      .Include(x => x.Collection.AccessCondition)
      .Include(x => x.Collection.CollectionType)
      .Include(x => x.Collection.AssociatedData)
          .ThenInclude(x => x.AssociatedDataType)
      .Include(x => x.Collection.AssociatedData)
          .ThenInclude(x => x.AssociatedDataProcurementTimeframe)
      .Include(x => x.AgeRange)
      .Include(x => x.DonorCount)
      .Include(x => x.Sex)
      .Include(x => x.MaterialDetails)
      .Include(x => x.Collection.Organisation.OrganisationServiceOfferings)
          .ThenInclude(x => x.ServiceOffering)
      .Include(x => x.MaterialDetails)
          .ThenInclude(x => x.CollectionPercentage)
      .Include(x => x.MaterialDetails)
          .ThenInclude(x => x.MacroscopicAssessment)
      .Include(x => x.MaterialDetails)
          .ThenInclude(x => x.MaterialType)
      .Include(x => x.MaterialDetails)
          .ThenInclude(x => x.StorageTemperature)
      .Include(x => x.Collection.Organisation.Country)
      .Include(x => x.Collection.Organisation.County)
      .FirstOrDefaultAsync()
      );

    private async Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexingAsync(
    IEnumerable<int> capabilityIds) => (
    await _context.DiagnosisCapabilities.Where(x =>
              capabilityIds.Contains(x.DiagnosisCapabilityId) && !x.Organisation.IsSuspended)
              .Include(x => x.Organisation)
              .Include(x => x.Organisation.OrganisationNetworks.Select(on => on.Network))
              .Include(x => x.Organisation.OrganisationServiceOfferings.Select(s => s.ServiceOffering))
              .Include(x => x.OntologyTerm)
              .Include(x => x.AssociatedData)
              .Include(x => x.SampleCollectionMode)
              .ToListAsync()
    );
    public async Task BuildIndex()
    {
      //Building the Search Index

      var searchBase = _elasticsearchConfig.ElasticsearchUrl;
      var indexNames = new Dictionary<string, string>
      {
        ["collections"] = _elasticsearchConfig.DefaultCollectionsSearchIndex,
        ["capabilities"] = _elasticsearchConfig.DefaultCapabilitiesSearchIndex,
      };

      var _navPaths = new List<string>()
            {
                Path.Combine(_hostEnvironment.ContentRootPath,@"~/App_Config/capabilities.json"),
                Path.Combine(_hostEnvironment.ContentRootPath,@"~/App_Config/collections.json")
            };
      using (var client = new HttpClient())
      {
        try
        {
          foreach (var path in _navPaths)
          {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var indexName = indexNames.ContainsKey(fileName)
                ? indexNames[fileName]
                : fileName;

            //Deleting the Index
            var deleteResponse = await client.DeleteAsync($"{searchBase}/{indexName}");

            //Creating the Index                      
            HttpContent pathContent = new StringContent(File.ReadAllText(path), System.Text.Encoding.UTF8, "application/json");
            var createResponse = await client.PutAsync($"{searchBase}/{indexName}", pathContent);

            //Preventing Index Replication              
            var indexString = "{ \"index\": { \"number_of_replicas\": 0 }}";
            HttpContent content = new StringContent(indexString, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{searchBase}/*/_settings", content);
          }
        }
        catch (Exception e) when (e is IOException || e is HttpRequestException)
        {
          _telemetryClient.TrackException(e);
          throw;
        }
      }
    }


    public async Task<string> GetClusterHealth()
    {
      var searchBase = _elasticsearchConfig.ElasticsearchUrl;

      try
      {
        using (var client = new HttpClient())
        {
          var response = await client.GetStringAsync($"{searchBase}/_cluster/health");
          var clusterHealth = JsonConvert.DeserializeAnonymousType(response, new
          {
            Status = string.Empty
          });

          return clusterHealth.Status;
        }
      }
      catch (Exception e)
      {
        // Log Error via Application Insights
        _telemetryClient.TrackException(e);
      }

      return null;
    }

    public async Task IndexSampleSet(int sampleSetId)
    {
      // Get the entire sample set object from the database.
      var createdSampleSet = await GetSampleSetByIdForIndexingAsync(sampleSetId);


      // Queue up a job to add the sample set to the search index.
      BackgroundJob.Enqueue(() => _indexProvider.IndexCollectionSearchDocument(
          createdSampleSet.Id,
          createdSampleSet.ToCollectionSearchDocument()));
    }

    public async Task IndexCapability(int capabilityId)
    {
      // Get the entire capability object from the database.
      var createdCapability = await _context.DiagnosisCapabilities
                .AsNoTracking()
                .Where(x => x.DiagnosisCapabilityId == capabilityId)
                .Include(x => x.Organisation)
                .Include(x => x.Organisation.OrganisationNetworks)
                  .ThenInclude(on => on.Network)
                .Include(x => x.Organisation.OrganisationServiceOfferings)
                  .ThenInclude(x => x.ServiceOffering)
                .Include(x => x.OntologyTerm)
                .Include(x => x.AssociatedData)
                  .ThenInclude(x => x.AssociatedDataType)
                .Include(x => x.AssociatedData)
                  .ThenInclude(x => x.AssociatedDataProcurementTimeframe)
                .Include(x => x.SampleCollectionMode)
                .FirstOrDefaultAsync();
            

      // Get the donor counts.
      var donorCounts = await _donorCountService.List();

      // Set up the capability search document.
      var capabilitySearchDocument = createdCapability.ToCapabilitySearchDocument(donorCounts.ToList());

      // Queue up a job to add the sample set to the search index.
      BackgroundJob.Enqueue(() => _indexProvider.IndexCapabilitySearchDocument(
          createdCapability.DiagnosisCapabilityId,
          capabilitySearchDocument));
    }

    public async Task UpdateSampleSetDetails(int sampleSetId)
    {
      // Get the entire sample set object from the database.
      var sampleSet = await GetSampleSetByIdForIndexingAsync(sampleSetId);

      // Partial Sample Set For Indexing
      var partialSampleSet = new PartialSampleSet
      {
        Sex = sampleSet.Sex.Value,
        AgeRange = sampleSet.AgeRange.Value,
        AgeRangeMetadata = JsonConvert.SerializeObject(new
        {
          Name = sampleSet.AgeRange.Value,
          sampleSet.AgeRange.SortOrder
        }),
        DonorCount = sampleSet.DonorCount.Value,
        DonorCountMetadata = JsonConvert.SerializeObject(new
        {
          Name = sampleSet.DonorCount.Value,
          sampleSet.DonorCount.SortOrder
        }),
        MaterialPreservationDetails =
                    sampleSet
                        .MaterialDetails
                        .Select(x => new MaterialPreservationDetailDocument
                        {
                          MaterialType = x.MaterialType.Value,
                          StorageTemperature = x.StorageTemperature.Value,
                          StorageTemperatureMetadata = JsonConvert.SerializeObject(new
                          {
                            Name = x.StorageTemperature.Value,
                            x.StorageTemperature.SortOrder
                          }),
                          MacroscopicAssessment = x.MacroscopicAssessment.Value,
                          PercentageOfSampleSet = x.CollectionPercentage?.Value
                        })
                        .ToList(),
        SampleSetSummary = SampleSetExtensions.BuildSampleSetSummary(
                        sampleSet.DonorCount.Value,
                        sampleSet.AgeRange.Value,
                        sampleSet.Sex.Value,
                        sampleSet.MaterialDetails)
      };

      // Queue up a job to update the sample set in the search index.
      BackgroundJob.Enqueue(() =>
          _indexProvider.UpdateCollectionSearchDocument(sampleSet.Id, partialSampleSet));
    }

    public void DeleteSampleSet(int sampleSetId)
    {
      // Queue up a job to remove the sample set from the search index.
      BackgroundJob.Enqueue(() => _indexProvider.DeleteCollectionSearchDocument(sampleSetId));
    }

    public void DeleteCapability(int capabilityId)
    {
      // Queue up a job to remove the capability from the search index.
      BackgroundJob.Enqueue(() => _indexProvider.DeleteCapabilitySearchDocument(capabilityId));
    }

    public void UpdateCollectionDetails(Collection collection)
    {
      // Update all search documents that are relevant to this collection.
      foreach (var sampleSet in collection.SampleSets)
      {
        // Queue up a job to update the search document.
        BackgroundJob.Enqueue(() =>
            _indexProvider.UpdateCollectionSearchDocument(
                sampleSet.Id,
                new PartialCollection
                {
                  OntologyTerm = collection.OntologyTerm.Value,
                  CollectionTitle = collection.Title,
                  StartYear = collection.StartDate.Year.ToString(),
                  CollectionStatus = collection.CollectionStatus.Value,
                  ConsentRestrictions = SampleSetExtensions.BuildConsentRestrictions(collection.ConsentRestrictions.ToList()),
                  AccessCondition = collection.AccessCondition.Value,
                  CollectionType = collection.CollectionType != null ? collection.CollectionType.Value : string.Empty,
                  AssociatedData = collection.AssociatedData.Select(ad => new AssociatedDataDocument
                  {
                    Text = ad.AssociatedDataType.Value,
                    Timeframe = ad.AssociatedDataProcurementTimeframe.Value,
                    TimeframeMetadata = JsonConvert.SerializeObject(new
                    {
                      Name = ad.AssociatedDataProcurementTimeframe.Value,
                      ad.AssociatedDataProcurementTimeframe.SortOrder
                    })
                  }),
                  OntologyOtherTerms = SampleSetExtensions.ParseOtherTerms(collection.OntologyTerm.OtherTerms)
                }));
      }
    }

    public void UpdateOrganisationDetails(Organisation biobank)
    {
      var partialBiobank = new PartialBiobank
      {
        Biobank = biobank.Name,
        BiobankServices = biobank.OrganisationServiceOfferings.Select(x => new BiobankServiceDocument
        {
          Name = x.ServiceOffering.Value
        })
      };

      // Update all collection search documents that are relevant to this biobank.
      foreach (var sampleSet in biobank.Collections.SelectMany(c => c.SampleSets))
      {
        // Queue up a job to update the search document.
        BackgroundJob.Enqueue(() =>
            _indexProvider.UpdateCollectionSearchDocument(
                sampleSet.Id,
                partialBiobank));
      }

      // Update all capability search documents that are relevant to this biobank.
      foreach (var capability in biobank.DiagnosisCapabilities)
      {
        // Queue up a job to update the search document.
        BackgroundJob.Enqueue(() =>
            _indexProvider.UpdateCapabilitySearchDocument(
                capability.DiagnosisCapabilityId,
                partialBiobank));
      }
    }

    public void UpdateNetwork(Network network)
    {
      // For all biobanks attached to this network.
      foreach (var biobank in network.OrganisationNetworks.Select(x => x.Organisation))
      {
        // Build the list of network documents.
        var networkDocuments = biobank.OrganisationNetworks
            .Select(on => on.Network)
            .Select(n => new NetworkDocument
            {
              Name = n.Name
            });

        // Update all search documents that are relevant to this biobank.
        foreach (var sampleSet in biobank.Collections.SelectMany(c => c.SampleSets))
        {
          // Queue up a job to update the search document.
          BackgroundJob.Enqueue(() =>
              _indexProvider.UpdateCollectionSearchDocument(
                  sampleSet.Id,
                  new PartialNetworks
                  {
                    Networks = networkDocuments
                  }));
        }

        // Update all search documents that are relevant to this biobank.
        foreach (var capability in biobank.DiagnosisCapabilities)
        {
          // Queue up a job to update the search document.
          BackgroundJob.Enqueue(() =>
              _indexProvider.UpdateCapabilitySearchDocument(
                  capability.DiagnosisCapabilityId,
                  new PartialNetworks
                  {
                    Networks = networkDocuments
                  }));
        }
      }
    }

    //TODO: Current unused method
    public void JoinOrLeaveNetwork(Organisation organisation)
    {
      // Update all search documents that are relevant to this biobank.
      foreach (var sampleSet in organisation.Collections.SelectMany(c => c.SampleSets))
      {
        // Build the list of network documents.
        var networkDocuments = organisation.OrganisationNetworks
            .Select(on => on.Network)
            .Select(n => new NetworkDocument
            {
              Name = n.Name
            });

        // Queue up a job to update the search document.
        BackgroundJob.Enqueue(() =>
            _indexProvider.UpdateCollectionSearchDocument(
                sampleSet.Id,
                new PartialNetworks
                {
                  Networks = networkDocuments
                }));
      }
    }

    public async Task BulkIndexBiobank(Organisation organisation)
    {
      //Index samplesets
      await
          BulkIndexSampleSets(
              organisation.Collections
                  .SelectMany(x => x.SampleSets)
                  .Select(x => x.Id)
                  .ToList());

      //Index capabilities
      await
          BulkIndexCapabilities(
              organisation.DiagnosisCapabilities
                  .Select(x => x.DiagnosisCapabilityId)
                  .ToList());
    }

    public async Task BulkIndexSampleSets(IList<int> sampleSetIds)
    {
      if (!(sampleSetIds?.Any() ?? false)) return;

      var chunkCount = GetChunkCount(sampleSetIds, BulkIndexChunkSize);
      var remainingIdCount = sampleSetIds.Count() % BulkIndexChunkSize;

      for (var i = 0; i < chunkCount; i++)
      {

        var chunkSampleSets = await GetSampleSetsByIdsForIndexingAsync(sampleSetIds
                .Skip(i * BulkIndexChunkSize)
                .Take(BulkIndexChunkSize));

        BackgroundJob.Enqueue(
            () => _indexProvider.BulkIndexCollectionSearchDocuments(chunkSampleSets
                .Select(x => x.ToCollectionSearchDocument())));
      }

      var remainingSampleSets = await GetSampleSetsByIdsForIndexingAsync(sampleSetIds
              .Skip(chunkCount * BulkIndexChunkSize)
              .Take(remainingIdCount));

      BackgroundJob.Enqueue(
          () => _indexProvider.BulkIndexCollectionSearchDocuments(remainingSampleSets
              .Select(x => x.ToCollectionSearchDocument())));
    }

    public async Task BulkIndexCapabilities(IList<int> capabilityIds)
    {
      if (!(capabilityIds?.Any() ?? false)) return;

      // Get the number of chunks of X records that we need to index.
      var chunkCount = GetChunkCount(capabilityIds, BulkIndexChunkSize);
      // Get the count of records that don't fit inside the neat chunks.
      var remainingIdCount = capabilityIds.Count() % BulkIndexChunkSize;

      // Get the donor counts.
      var donorCounts = (await _donorCountService.List()).ToList();

      for (var i = 0; i < chunkCount; i++)
      {
        var chunkSampleSets = await GetCapabilitiesByIdsForIndexingAsync
            (capabilityIds
                .Skip(i * BulkIndexChunkSize)
                .Take(BulkIndexChunkSize));

        BackgroundJob.Enqueue(
            () => _indexProvider.BulkIndexCapabilitySearchDocuments(chunkSampleSets
                .Select(x => x.ToCapabilitySearchDocument(donorCounts))));
      }

      var remainingSampleSets = await GetCapabilitiesByIdsForIndexingAsync
        (capabilityIds
              .Skip(chunkCount * BulkIndexChunkSize)
              .Take(remainingIdCount));

      BackgroundJob.Enqueue(
          () => _indexProvider.BulkIndexCapabilitySearchDocuments(remainingSampleSets
              .Select(x => x.ToCapabilitySearchDocument(donorCounts))));
    }

    public void BulkDeleteBiobank(Organisation organisation)
    {
      //Remove samplesets from the index
      BulkDeleteSampleSets(
              organisation.Collections
                  .SelectMany(x => x.SampleSets)
                  .Select(x => x.Id)
                  .ToList());

      //Remove capabilities from the index
      BulkDeleteCapabilities(
              organisation.DiagnosisCapabilities
                  .Select(x => x.DiagnosisCapabilityId)
                  .ToList());
    }

    public void BulkDeleteSampleSets(IList<int> sampleSetIds)
    {
      if (!(sampleSetIds?.Any() ?? false)) return;

      var chunkCount = GetChunkCount(sampleSetIds, BulkIndexChunkSize);
      var remainingIdCount = sampleSetIds.Count() % BulkIndexChunkSize;

      for (var i = 0; i < chunkCount; i++)
      {
        var chunkSampleSets = sampleSetIds
                .Skip(i * BulkIndexChunkSize)
                .Take(BulkIndexChunkSize);

        BackgroundJob.Enqueue(
            () => _indexProvider.BulkDeleteCollectionSearchDocuments(chunkSampleSets));
      }

      var remainingSampleSets = sampleSetIds
              .Skip(chunkCount * BulkIndexChunkSize)
              .Take(remainingIdCount);

      BackgroundJob.Enqueue(
          () => _indexProvider.BulkDeleteCollectionSearchDocuments(remainingSampleSets));
    }

    public void BulkDeleteCapabilities(IList<int> capabilityIds)
    {
      if (!(capabilityIds?.Any() ?? false)) return;

      // Get the number of chunks of X records that we need to index.
      var chunkCount = GetChunkCount(capabilityIds, BulkIndexChunkSize);
      // Get the count of records that don't fit inside the neat chunks.
      var remainingIdCount = capabilityIds.Count() % BulkIndexChunkSize;

      for (var i = 0; i < chunkCount; i++)
      {
        var chunkSampleSets = capabilityIds
                .Skip(i * BulkIndexChunkSize)
                .Take(BulkIndexChunkSize);

        BackgroundJob.Enqueue(
            () => _indexProvider.BulkDeleteCapabilitySearchDocuments(chunkSampleSets));
      }

      var remainingSampleSets = capabilityIds
              .Skip(chunkCount * BulkIndexChunkSize)
              .Take(remainingIdCount);

      BackgroundJob.Enqueue(
          () => _indexProvider.BulkDeleteCapabilitySearchDocuments(remainingSampleSets));
    }

    public async Task ClearIndex()
    {
      //Bulk delete every item in the index, by type
      BulkDeleteSampleSets(await _searchProvider.GetAllSampleSetIds());
      BulkDeleteCapabilities(await _searchProvider.GetAllCapabilityIds());
    }

    private static int GetChunkCount(IEnumerable<int> intList, int chunkSize)
        => (int)Math.Floor((double)(intList.Count() / chunkSize));

  }
}
