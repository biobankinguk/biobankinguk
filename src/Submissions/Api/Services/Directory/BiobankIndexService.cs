using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Search.Dto.Documents;
using Biobanks.Search.Dto.PartialDocuments;
using Biobanks.Search.Legacy;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.Directory.Extensions;
using Hangfire;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class BiobankIndexService : IBiobankIndexService
    {
        private const int BulkIndexChunkSize = 100;

        private readonly IReferenceDataService<DonorCount> _donorCountService;
        private readonly CapabilityService _capabilityService;

        private readonly IBiobankReadService _biobankReadService;
        private readonly IIndexProvider _indexProvider;
        private readonly ISearchProvider _searchProvider;

        private readonly IHostEnvironment _hostEnvironment;

        private readonly TelemetryClient _telemetryClient;

        public BiobankIndexService(
            IReferenceDataService<DonorCount> donorCountService,
            CapabilityService capabilityService,
            IBiobankReadService biobankReadService,
            IIndexProvider indexProvider,
            ISearchProvider searchProvider,
            IHostEnvironment hostEnvironment,
            TelemetryClient telemetryClient)
        {
            _donorCountService = donorCountService;
            _capabilityService = capabilityService;
            _biobankReadService = biobankReadService;
            _indexProvider = indexProvider;
            _searchProvider = searchProvider;
            _hostEnvironment = hostEnvironment;
            _telemetryClient = telemetryClient;
        }

        public async Task BuildIndex()
        {
            //Building the Search Index

            var searchBase = ConfigurationManager.AppSettings["ElasticSearchUrl"];
            var indexNames = new Dictionary<string, string>
            {
                ["collections"] = ConfigurationManager.AppSettings["DefaultCollectionsSearchIndex"],
                ["capabilities"] = ConfigurationManager.AppSettings["DefaultCapabilitiesSearchIndex"]
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
            var searchBase = ConfigurationManager.AppSettings["ElasticSearchUrl"];

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
            var createdSampleSet = await _biobankReadService.GetSampleSetByIdForIndexingAsync(sampleSetId);

            // Queue up a job to add the sample set to the search index.
            BackgroundJob.Enqueue(() => _indexProvider.IndexCollectionSearchDocument(
                createdSampleSet.Id,
                createdSampleSet.ToCollectionSearchDocument()));
        }

        public async Task IndexCapability(int capabilityId)
        {
            // Get the entire capability object from the database.
            var createdCapability = await _capabilityService.GetCapabilityByIdForIndexingAsync(capabilityId);

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
            var sampleSet = await _biobankReadService.GetSampleSetByIdForIndexingAsync(sampleSetId);

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
                var chunkSampleSets = await _biobankReadService
                    .GetSampleSetsByIdsForIndexingAsync(sampleSetIds
                        .Skip(i * BulkIndexChunkSize)
                        .Take(BulkIndexChunkSize));

                BackgroundJob.Enqueue(
                    () => _indexProvider.BulkIndexCollectionSearchDocuments(chunkSampleSets
                        .Select(x => x.ToCollectionSearchDocument())));
            }

            var remainingSampleSets = await _biobankReadService
                .GetSampleSetsByIdsForIndexingAsync(sampleSetIds
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
                var chunkSampleSets = await _capabilityService
                    .GetCapabilitiesByIdsForIndexingAsync(capabilityIds
                        .Skip(i * BulkIndexChunkSize)
                        .Take(BulkIndexChunkSize));

                BackgroundJob.Enqueue(
                    () => _indexProvider.BulkIndexCapabilitySearchDocuments(chunkSampleSets
                        .Select(x => x.ToCapabilitySearchDocument(donorCounts))));
            }

            var remainingSampleSets = await _capabilityService
                .GetCapabilitiesByIdsForIndexingAsync(capabilityIds
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
