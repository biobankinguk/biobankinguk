﻿using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Entities.Api;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Shared.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class AggregatorJob
    {
        private const string FitAndWell = "102499006";

        private readonly IAggregationService _aggregationService;
        private readonly IReferenceDataService _refDataService;
        private readonly IOrganisationService _organisationService;
        private readonly ICollectionService _collectionService;
        private readonly ISampleService _sampleService;

        public AggregatorJob(
            IAggregationService aggregationService,
            IReferenceDataService refDataService,
            IOrganisationService organisationService,
            ICollectionService collectionService,
            ISampleService sampleService)
        {
            _aggregationService = aggregationService;
            _refDataService = refDataService;
            _organisationService = organisationService;
            _collectionService = collectionService;
            _sampleService = sampleService;
        }

        public async Task Run()
        {
            // All Extracted Samples Flagged For Update/Deletion
            var dirtyExtractedSamples = await _sampleService.ListDirtyExtractedSamples();
            var dirtyNonExtractedSamples = await _sampleService.ListDirtyNonExtractedSamples();

            // Delete Samples With isDeleted Flag
            await _sampleService.DeleteFlaggedSamples();

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

            // All Reminaing Non-Extracted Samples Aggregated Under 'Fit and Well'
            var fitAndWell = _refDataService.GetOntologyTerm(FitAndWell);

            foreach (var collectionSamples in _aggregationService.GroupIntoCollections(dirtyNonExtractedSamples))
            {
                var baseSample = new LiveSample
                {
                    SampleContent = fitAndWell,
                    OrganisationId = collectionSamples.First().OrganisationId
                };

                // Remaining Maybe Empty If All Dirty Non-Extracted Samples Were Deleted
                var samples = remainingSamples.Where(x => x.OrganisationId == baseSample.OrganisationId);

                // Aggregate Under 'Fit and Well'
                await AggregateCollectionSamples(baseSample, samples);
            }
        }

        private async Task AggregateCollectionSamples(LiveSample baseSample, IEnumerable<LiveSample> samples)
        {
            var organisation = await _organisationService.GetById(baseSample.OrganisationId);

            // Find Exisiting Or Generate New Collection
            var collectionName = _aggregationService.GenerateCollectionName(baseSample);
            var collection = 
                await _collectionService.GetCollection(organisation.OrganisationId, collectionName) 
                ?? _aggregationService.GenerateCollection(samples, collectionName);

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
                // TODO: Use Proper Cascade Deletion
                await _collectionService.DeleteMaterialDetailsBySampleSetIds(collection.SampleSets.Select(x => x.Id));
                await _collectionService.DeleteSampleSetByIds(collection.SampleSets.Select(x => x.Id));
                await _collectionService.DeleteCollection(collection.CollectionId);
            }

            // Flag These Samples As Clean
            await _sampleService.CleanSamples(samples);
        }
    }
}
