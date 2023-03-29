using Biobanks.Aggregator;
using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Entities.Api;
using Biobanks.Shared.Services.Contracts;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class AggregatorJob
    {
        private readonly IAggregationService _aggregationService;
        private readonly IReferenceDataAggregatorService _refDataService;
        private readonly IOrganisationService _organisationService;
        private readonly ICollectionAggregatorService _collectionService;
        private readonly ISampleService _sampleService;

        private readonly AggregatorOptions _options;

        public AggregatorJob(
            IAggregationService aggregationService,
            IReferenceDataAggregatorService refDataService,
            IOrganisationService organisationService,
            ICollectionAggregatorService collectionService,
            ISampleService sampleService,
            IOptions<AggregatorOptions> options)
        {
            _aggregationService = aggregationService;
            _refDataService = refDataService;
            _organisationService = organisationService;
            _collectionService = collectionService;
            _sampleService = sampleService;
            _options = options.Value;
        }

        public async Task Run()
        {
            // All Extracted Samples Flagged For Update/Deletion
            var dirtyExtractedSamples = await _sampleService.ListDirtyExtractedSamples();
            var dirtyNonExtractedSamples = await _sampleService.ListDirtyNonExtractedSamples();

            // Aggregate Extracted Samples
            foreach (var collectionSamples in _aggregationService.GroupIntoCollections(dirtyExtractedSamples))
            {
                // Include Relevant Non-Extracted Samples
                var samples = await _sampleService.ListSimilarSamples(collectionSamples);

                await AggregateCollectionSamples(
                    baseSample: collectionSamples.First(), 
                    samples);
            }

            // Aggregate Remaining Non-Extracted Samples
            var remainingSamples = await _sampleService.ListDirtyNonExtractedSamples();

            foreach (var collectionSamples in _aggregationService.GroupIntoCollections(dirtyNonExtractedSamples))
            {
                var baseSample = new LiveSample
                {
                    OrganisationId = collectionSamples.First().OrganisationId,
                    SampleContentId = _options.NonExtractedOntologyTerm,
                    SampleContent = _refDataService.GetOntologyTerm(_options.NonExtractedOntologyTerm)
                };

                // Remaining Maybe Empty If All Dirty Non-Extracted Samples Were Deleted
                var samples = remainingSamples
                    .Where(x => x.OrganisationId == baseSample.OrganisationId)
                    .Where(x => !x.IsDeleted);

                // Aggregate Under 'Fit and Well'
                await AggregateCollectionSamples(baseSample, samples);
            }

            // Finally Delete Samples With isDeleted Flag
            await _sampleService.DeleteFlaggedSamples();
        }

        private async Task AggregateCollectionSamples(LiveSample baseSample, IEnumerable<LiveSample> samples)
        {
            if (baseSample == null || baseSample.SampleContent == null)
                throw new ArgumentException("Supplied baseSample is null, or is referencing a non-existent SampleContent");

            var organisation = await _organisationService.GetById(baseSample.OrganisationId);
            var collectionName = _aggregationService.GenerateCollectionName(baseSample);

            // Check Organisation Has Properly Configured Default Values
            if (organisation.AccessCondition is null || organisation.CollectionType is null)
            {
                // Log Warning
                Console.WriteLine($"Could not aggregate {collectionName} for {organisation.Name} due to missing default aggregation values.");

                // Return As We Can Not Aggregate These Samples
                return;
            }

            // Find Exisiting Or Generate New Collection
            var collection = await _collectionService.GetCollection(organisation.OrganisationId, collectionName);

            // Generate Collection If None Existing
            if (collection is null)
            {
                collection = _aggregationService.GenerateCollection(samples, collectionName);

                if (string.IsNullOrEmpty(collection.OntologyTermId))
                {
                    collection.OntologyTermId = baseSample.SampleContentId;
                }
            }
                
            if (samples.Any())
            {
                // Update Collection Contextual Fields
                collection.LastUpdated = DateTime.Now;
                collection.CollectionTypeId = organisation.CollectionTypeId;
                collection.AccessConditionId = organisation.AccessConditionId ?? 0;

                // Clear Current SampleSets - Rebuilt Below
                collection.SampleSets.Clear();

                // Group Samples Into SampleSets
                foreach (var sampleSetSamples in _aggregationService.GroupIntoSampleSets(samples))
                {
                    var sampleSet = _aggregationService.GenerateSampleSet(sampleSetSamples);

                    // Group Samples Into MaterialDetails
                    foreach (var materialDetailSamples in _aggregationService.GroupIntoMaterialDetails(sampleSetSamples))
                    {
                        var materialDetail = _aggregationService.GenerateMaterialDetail(materialDetailSamples);

                        // Set Collection Percentage For Material Detail
                        materialDetail.CollectionPercentage =
                            _refDataService.GetCollectionPercentage(
                                100m * materialDetailSamples.Count() / sampleSetSamples.Count());

                        sampleSet.MaterialDetails.Add(materialDetail);
                    }

                    collection.SampleSets.Add(sampleSet);
                }

                // Write Collection To DB
                if (collection.CollectionId == default)
                {
                    await _collectionService.AddCollection(collection);
                }
                else
                {
                    await _collectionService.UpdateCollection(collection);
                }
            }
            else
            {
                await _collectionService.DeleteCollection(collection.CollectionId);
            }

            // Flag These Samples As Clean
            await _sampleService.CleanSamples(samples);
        }
    }
}
