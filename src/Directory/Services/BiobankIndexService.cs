using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Search.Legacy;
using Castle.Core.Internal;
using Hangfire;
using Newtonsoft.Json;
using Biobanks.Search.Dto.PartialDocuments;
using Biobanks.Search.Dto.Documents;
using System.Configuration;
using System.Net.Http;
using Biobanks.Services.Contracts;
using Biobanks.Services.Extensions;
using System.IO;
using System.Web.Hosting;
using Microsoft.ApplicationInsights;

namespace Biobanks.Services
{
    public class BiobankIndexService : IBiobankIndexService
    {
        private const int BulkIndexChunkSize = 100;

        private readonly IBiobankReadService _biobankReadService;
        private readonly IIndexProvider _indexProvider;
        private readonly ISearchProvider _searchProvider;

        public BiobankIndexService(
            IBiobankReadService biobankReadService,
            IIndexProvider indexProvider,
            ISearchProvider searchProvider)
        {
            _biobankReadService = biobankReadService;
            _indexProvider = indexProvider;
            _searchProvider = searchProvider;
        }

        public async Task BuildIndex()
        {
            //Building the Search Index

            var searchBase = ConfigurationManager.AppSettings["ElasticSearchUrl"];
            var _navPaths = new List<string>()
            {
                HostingEnvironment.MapPath(@"~/App_Config/capabilities.json"),
                HostingEnvironment.MapPath(@"~/App_Config/collections.json")
            };
            using (var client = new HttpClient())
            {
                try
                {
                    foreach (var path in _navPaths)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(path);
                    
                        //Deleting the Index
                        var deleteResponse = await client.DeleteAsync($"{searchBase}/{fileName}");

                        //Creating the Index                      
                        HttpContent pathContent = new StringContent(System.IO.File.ReadAllText(path), System.Text.Encoding.UTF8, "application/json");
                        var createResponse = await client.PutAsync($"{searchBase}/{fileName}", pathContent);

                        //Preventing Index Replication              
                        var indexString = "{ \"index\": { \"number_of_replicas\": 0 }}";
                        HttpContent content = new StringContent(indexString, System.Text.Encoding.UTF8, "application/json");
                        var response = await client.PutAsync($"{searchBase}/*/_settings", content);
                    }                
                }
                catch (Exception e) when (e is IOException || e is HttpRequestException)
                {                           
                    var ai = new TelemetryClient();
                    ai.TrackException(e);
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
                var ai = new TelemetryClient();
                ai.TrackException(e);
            }

            return null;
        }

        public async Task IndexSampleSet(int sampleSetId)
        {
            // Get the entire sample set object from the database.
            var createdSampleSet = await _biobankReadService.GetSampleSetByIdForIndexingAsync(sampleSetId);

            // Queue up a job to add the sample set to the search index.
            BackgroundJob.Enqueue(() => _indexProvider.IndexCollectionSearchDocument(
                createdSampleSet.SampleSetId,
                createdSampleSet.ToCollectionSearchDocument()));
        }

        public async Task IndexCapability(int capabilityId)
        {
            // Get the entire capability object from the database.
            var createdCapability = await _biobankReadService.GetCapabilityByIdForIndexingAsync(capabilityId);

            // Get the donor counts.
            var donorCounts = (await _biobankReadService.ListDonorCountsAsync()).ToList();

            // Set up the capability search document.
            var capabilitySearchDocument = createdCapability.ToCapabilitySearchDocument(donorCounts);

            // Queue up a job to add the sample set to the search index.
            BackgroundJob.Enqueue(() => _indexProvider.IndexCapabilitySearchDocument(
                createdCapability.DiagnosisCapabilityId,
                capabilitySearchDocument));
        }

        public async Task UpdateSampleSetDetails(int sampleSetId)
        {
            // Get the entire sample set object from the database.
            var updatedSampleSet = await _biobankReadService.GetSampleSetByIdForIndexingAsync(sampleSetId);

            // Queue up a job to update the sample set in the search index.
            BackgroundJob.Enqueue(() => _indexProvider.UpdateCollectionSearchDocument(
                updatedSampleSet.SampleSetId,
                new PartialSampleSet
                {
                    Sex = updatedSampleSet.Sex.Value,
                    AgeRange = updatedSampleSet.AgeRange.Value,
                    AgeRangeMetadata = JsonConvert.SerializeObject(new
                    {
                        Name = updatedSampleSet.AgeRange.Value,
                        updatedSampleSet.AgeRange.SortOrder
                    }),
                    DonorCount = updatedSampleSet.DonorCount.Value,
                    DonorCountMetadata = JsonConvert.SerializeObject(new
                    {
                        Name = updatedSampleSet.DonorCount.Value,
                        updatedSampleSet.DonorCount.SortOrder
                    }),
                    MaterialPreservationDetails = updatedSampleSet.MaterialDetails
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
                            PercentageOfSampleSet = x.CollectionPercentage.Value
                        }),
                    SampleSetSummary = SampleSetExtensions.BuildSampleSetSummary(
                        updatedSampleSet.DonorCount.Value, 
                        updatedSampleSet.AgeRange.Value,
                        updatedSampleSet.Sex.Value,
                        updatedSampleSet.MaterialDetails)
                }));
        }

        public async Task UpdateCapabilityDetails(int capabilityId)
        {
            // Get the entire capability object from the database.
            var updatedCapability = await _biobankReadService.GetCapabilityByIdForIndexingAsync(capabilityId);

            // Get the donor counts and get expectations from them
            var donorExpectation = DiagnosisCapabilityExtensions.GetAnnualDonorExpectationRange(
                        await _biobankReadService.ListDonorCountsAsync(),
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

        public async Task UpdateCollectionDetails(int collectionId)
        {
            // Get the collection out of the database.
            var collection = await _biobankReadService.GetCollectionByIdForIndexingAsync(collectionId);

            // Update all search documents that are relevant to this collection.
            foreach (var sampleSet in collection.SampleSets)
            {
                // Queue up a job to update the search document.
                BackgroundJob.Enqueue(() =>
                    _indexProvider.UpdateCollectionSearchDocument(
                        sampleSet.SampleSetId,
                        new PartialCollection
                        {
                            OntologyTerm = collection.OntologyTerm.Value,
                            CollectionTitle = collection.Title,
                            StartYear = collection.StartDate.Year.ToString(),
                            CollectionPoint = collection.CollectionPoint.Value,
                            CollectionStatus = collection.CollectionStatus.Value,
                            ConsentRestrictions = SampleSetExtensions.BuildConsentRestrictions(collection.ConsentRestrictions.ToList()),
                            HTA = collection.HtaStatus != null ? collection.HtaStatus.Value : "not recorded",
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

        public async Task UpdateBiobankDetails(int biobankId)
        {
            // Get the biobank from the database.
            var biobank = await _biobankReadService.GetBiobankByIdForIndexingAsync(biobankId);

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
                        sampleSet.SampleSetId,
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

        public async Task UpdateNetwork(int networkId)
        {
            // For all biobanks attached to this network.
            foreach (var biobank in await _biobankReadService.GetBiobanksByNetworkIdForIndexingAsync(networkId))
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
                           sampleSet.SampleSetId,
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

        public async Task JoinOrLeaveNetwork(int biobankId)
        {
            // Get the biobank from the database.
            var biobank = await _biobankReadService.GetBiobankByIdForIndexingAsync(biobankId);

            // Update all search documents that are relevant to this biobank.
            foreach (var sampleSet in biobank.Collections.SelectMany(c => c.SampleSets))
            {
                // Build the list of network documents.
                var networkDocuments = biobank.OrganisationNetworks
                    .Select(on => on.Network)
                    .Select(n => new NetworkDocument
                    {
                        Name = n.Name
                    });

                // Queue up a job to update the search document.
                BackgroundJob.Enqueue(() =>
                    _indexProvider.UpdateCollectionSearchDocument(
                        sampleSet.SampleSetId,
                        new PartialNetworks
                        {
                            Networks = networkDocuments
                        }));
            }
        }

        public async Task BulkIndexBiobank(int biobankId)
        {
            //Get the biobank, complete with collections, samplesets, capabilities
            var biobank = await _biobankReadService.GetBiobankByIdForIndexingAsync(biobankId);

            //Index samplesets
            await
                BulkIndexSampleSets(
                    biobank.Collections
                        .SelectMany(x => x.SampleSets)
                        .Select(x => x.SampleSetId)
                        .ToList());

            //Index capabilities
            await
                BulkIndexCapabilities(
                    biobank.DiagnosisCapabilities
                        .Select(x => x.DiagnosisCapabilityId)
                        .ToList());
        }

        public async Task BulkIndexSampleSets(IList<int> sampleSetIds)
        {
            if (sampleSetIds.IsNullOrEmpty()) return;

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
            if (capabilityIds.IsNullOrEmpty()) return;

            // Get the number of chunks of X records that we need to index.
            var chunkCount = GetChunkCount(capabilityIds, BulkIndexChunkSize);
            // Get the count of records that don't fit inside the neat chunks.
            var remainingIdCount = capabilityIds.Count() % BulkIndexChunkSize;

            // Get the donor counts.
            var donorCounts = (await _biobankReadService.ListDonorCountsAsync()).ToList();

            for (var i = 0; i < chunkCount; i++)
            {
                var chunkSampleSets = await _biobankReadService
                    .GetCapabilitiesByIdsForIndexingAsync(capabilityIds
                        .Skip(i * BulkIndexChunkSize)
                        .Take(BulkIndexChunkSize));

                BackgroundJob.Enqueue(
                    () => _indexProvider.BulkIndexCapabilitySearchDocuments(chunkSampleSets
                        .Select(x => x.ToCapabilitySearchDocument(donorCounts))));
            }

            var remainingSampleSets = await _biobankReadService
                .GetCapabilitiesByIdsForIndexingAsync(capabilityIds
                    .Skip(chunkCount * BulkIndexChunkSize)
                    .Take(remainingIdCount));

            BackgroundJob.Enqueue(
                () => _indexProvider.BulkIndexCapabilitySearchDocuments(remainingSampleSets
                    .Select(x => x.ToCapabilitySearchDocument(donorCounts))));
        }

        public async Task BulkDeleteBiobank(int biobankId)
        {
            //Get the biobank, complete with collections, samplesets, capabilities
            var biobank = await _biobankReadService.GetBiobankByIdForIndexingAsync(biobankId);

            //Remove samplesets from the index
            BulkDeleteSampleSets(
                    biobank.Collections
                        .SelectMany(x => x.SampleSets)
                        .Select(x => x.SampleSetId)
                        .ToList());

            //Remove capabilities from the index
            BulkDeleteCapabilities(
                    biobank.DiagnosisCapabilities
                        .Select(x => x.DiagnosisCapabilityId)
                        .ToList());
        }

        public void BulkDeleteSampleSets(IList<int> sampleSetIds)
        {
            if (sampleSetIds.IsNullOrEmpty()) return;

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
            if (capabilityIds.IsNullOrEmpty()) return;

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
            => (int) Math.Floor((double) (intList.Count() / chunkSize));

        public async Task UpdateCollectionsOntologyOtherTerms(string ontologyTerm)
        {
            // Get the collections with the ontologyTerm.
            var collectionIds = await _biobankReadService.GetCollectionIdsByOntologyTermAsync(ontologyTerm);
            // Update all search documents that are relevant to this collection.
            foreach (var collectionId in collectionIds)
            {
                await UpdateCollectionDetails(collectionId);
            }
        }

        public async Task UpdateCapabilitiesOntologyOtherTerms(string ontologyTerm)
        {
            // Get the capabilitiess with the ontologyTerm.
            var capabilityIds = await _biobankReadService.GetCapabilityIdsByOntologyTermAsync(ontologyTerm);
            // Update all search documents that are relevant to this collection.
            foreach (var capabilityId in capabilityIds)
            {
                await UpdateCapabilityDetails(capabilityId);
            }
        }
    }
}
