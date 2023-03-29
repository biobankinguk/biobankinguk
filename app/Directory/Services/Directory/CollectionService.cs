using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Search.Dto.Documents;
using Biobanks.Search.Dto.PartialDocuments;
using Biobanks.Search.Legacy;
using Biobanks.Submissions.Api.Services.Directory.Extensions;
using Hangfire;
using Newtonsoft.Json;

namespace Biobanks.Submissions.Api.Services.Directory
{
    /// <inheritdoc/>
    public class CollectionService : ICollectionService
    {

        private readonly IIndexProvider _indexProvider;

        private readonly ApplicationDbContext _db;

        public CollectionService(
            ApplicationDbContext db,
            IIndexProvider indexProvider)
        {
            _db = db;
            _indexProvider = indexProvider;
        }

        /// <inheritdoc/>
        public async Task<bool> Delete(int id)
        {
            if (await HasSampleSets(id))
                return false;

            var collection = await _db.Collections.FindAsync(id);
            _db.Collections.Remove(collection);

            await _db.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<Collection> Add(Collection collection)
        {
            // Match Consent Restritions
            var consentIds = collection.ConsentRestrictions?.Select(x => x.Id) ?? Enumerable.Empty<int>();

            collection.ConsentRestrictions = await _db.ConsentRestrictions
                .Where(x => consentIds.Contains(x.Id))
                .ToListAsync();

            // Update Timestamp
            collection.LastUpdated = DateTime.Now;

            _db.Collections.Add(collection);

            await _db.SaveChangesAsync();

            return collection;
        }


        /// <inheritdoc/>
        public async Task<Collection> Copy(int id, int biobankId)
        {
            var collectionToCopy = await Get(id);

            // get all collections to determine whether new ttle is already exists
            var collections = await List();

            var newTitle = "";

            if (string.IsNullOrEmpty(collectionToCopy.Title))
            {
                newTitle = collectionToCopy.OntologyTerm.Value;
            }
            else
            {
                newTitle = collectionToCopy.Title;
            }

            var index = 1;

            // create new name of collection. Pattern is 'oldName (Copy 1)' etc.
            while (true)
            {
                var tmpTitle = " (Copy " + index + ")";

                // check if there a collection with created title
                var titleExists = collections
                   .Where(x => x.Title == newTitle + tmpTitle)
                   .Select(x => x.Title)
                   .Distinct();

                // if title already exists, keep creating new title and check again
                if (titleExists.Any())
                {
                    index++;
                }
                else
                {
                    newTitle += tmpTitle;
                    break;
                }
            }


            var newCollection = new Collection
            {
                OrganisationId = biobankId,
                Title = newTitle,
                CollectionTypeId = collectionToCopy.CollectionTypeId,
                Description = collectionToCopy.Description,
                AssociatedData = collectionToCopy.AssociatedData
                    .Select(y => new CollectionAssociatedData
                    {
                        AssociatedDataTypeId = y.AssociatedDataTypeId,
                        AssociatedDataProcurementTimeframeId = y.AssociatedDataProcurementTimeframeId // GroupID
                    })
                    .ToList(),
                StartDate = new DateTime(year: collectionToCopy.StartDate.Year, month: 1, day: 1),
                AccessConditionId = collectionToCopy.AccessConditionId,
                LastUpdated = DateTime.Now,
                CollectionStatusId = collectionToCopy.CollectionStatusId,
                ConsentRestrictions = collectionToCopy.ConsentRestrictions,
                OntologyTermId = collectionToCopy.OntologyTermId,
                FromApi = false
            };

            // Reference Exisiting Consent Restrictions
            var consentRestrictionIds = newCollection.ConsentRestrictions?.Select(x => x.Id) ?? Enumerable.Empty<int>();

            newCollection.ConsentRestrictions?.Clear();
            newCollection.ConsentRestrictions = await _db.ConsentRestrictions
                .Where(x => consentRestrictionIds.Contains(x.Id))
                .ToListAsync();

            _db.Collections.Add(newCollection);

            await _db.SaveChangesAsync();

            // Index Updated Collection
            if (collectionToCopy.SampleSets != null && collectionToCopy.SampleSets.Any())
            {
                await UpdateCollectionDetails(newCollection.CollectionId);
            }

            return newCollection;
        }


        /// <inheritdoc/>
        public async Task<Collection> Update(Collection collection)
        {
            var currentCollection = await _db.Collections
                .Include(x => x.AssociatedData)
                .Include(x => x.ConsentRestrictions)
                .Include(x => x.Organisation)
                .FirstOrDefaultAsync(x => x.CollectionId == collection.CollectionId);

            if (currentCollection != null)
            {
                currentCollection.OntologyTermId = collection.OntologyTermId;
                currentCollection.Title = collection.Title;
                currentCollection.Description = collection.Description;
                currentCollection.StartDate = collection.StartDate;
                currentCollection.AccessConditionId = collection.AccessConditionId;
                currentCollection.CollectionTypeId = collection.CollectionTypeId;
                currentCollection.CollectionStatusId = collection.CollectionStatusId;
                currentCollection.LastUpdated = DateTime.Now;

                // Clear Old Associated Data In Favour Of New Data
                currentCollection.AssociatedData?.Clear();
                currentCollection.AssociatedData = collection.AssociatedData;

                // Reference Exisiting Consent Restrictions
                var consentRestrictionIds = collection.ConsentRestrictions?.Select(x => x.Id) ?? Enumerable.Empty<int>();

                currentCollection.ConsentRestrictions?.Clear();
                currentCollection.ConsentRestrictions = await _db.ConsentRestrictions
                    .Where(x => consentRestrictionIds.Contains(x.Id))
                    .ToListAsync();

                await _db.SaveChangesAsync();

                // Index Updated Collection
                if (!currentCollection.Organisation.IsSuspended)
                {
                    await UpdateCollectionDetails(currentCollection.CollectionId);
                }
            }

            return collection;
        }

        /// <inheritdoc/>
        public async Task<Collection> Get(int id)
            => await _db.Collections
                .AsNoTracking()
                .Include(x => x.AccessCondition)
                .Include(x => x.AssociatedData)
                .Include(x => x.CollectionType)
                .Include(x => x.CollectionStatus)
                .Include(x => x.ConsentRestrictions)
                .Include(x => x.OntologyTerm)
                .Include(x => x.SampleSets)
                    .ThenInclude(x => x.MaterialDetails)
                .FirstOrDefaultAsync(x => x.CollectionId == id);

        /// <inheritdoc/>
        public async Task<Collection> GetWithSampleSets(int id)
            => await _db.Collections
                .AsNoTracking()
                .Include(x => x.AccessCondition)
                .Include(x => x.AssociatedData)
                    .ThenInclude(x => x.AssociatedDataType)
                .Include(x => x.AssociatedData)
                    .ThenInclude(x => x.AssociatedDataProcurementTimeframe)
                .Include(x => x.CollectionType)
                .Include(x => x.CollectionStatus)
                .Include(x => x.ConsentRestrictions)
                .Include(x => x.OntologyTerm)
                .Include(x => x.SampleSets)
                    .ThenInclude(x => x.AgeRange)
                .Include(x => x.SampleSets)
                    .ThenInclude(x => x.Sex)
                .Include(x => x.SampleSets)
                    .ThenInclude(x => x.MaterialDetails)
                        .ThenInclude(x => x.MaterialType)
                .Include(x => x.SampleSets)
                    .ThenInclude(x => x.MaterialDetails)
                        .ThenInclude(x => x.StorageTemperature)                
                .Include(x => x.SampleSets)
                    .ThenInclude(x => x.MaterialDetails)
                        .ThenInclude(x => x.ExtractionProcedure)
                .Include(x => x.SampleSets)
                    .ThenInclude(x => x.MaterialDetails)
                        .ThenInclude(x => x.PreservationType)
                .FirstOrDefaultAsync(x => x.CollectionId == id);

        /// <inheritdoc/>
        public async Task<Collection> GetForIndexing(int id)
            => await _db.Collections
                .AsNoTracking()
                .Include(x => x.AccessCondition)
                .Include(x => x.AssociatedData)
                .Include(x => x.AssociatedData.Select(y => y.AssociatedDataType))
                .Include(x => x.AssociatedData.Select(y => y.AssociatedDataProcurementTimeframe))
                .Include(x => x.CollectionType)
                .Include(x => x.CollectionStatus)
                .Include(x => x.ConsentRestrictions)
                .Include(x => x.OntologyTerm)
                .Include(x => x.SampleSets)
                .FirstOrDefaultAsync(x => x.CollectionId == id);

        /// <inheritdoc/>
        public async Task<IEnumerable<Collection>> List(int organisationId = default)
            => await _db.Collections
                .AsNoTracking()
                .Include(x => x.Organisation)
                .Include(x => x.OntologyTerm)
                .Include(x => x.SampleSets)
                    .ThenInclude(x => x.MaterialDetails)
                        .ThenInclude(x => x.MaterialType)
                .Where(x => x.OrganisationId == organisationId || organisationId == default)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Collection>> ListByOntologyTerm(string ontologyTerm)
            => await _db.Collections
                .AsNoTracking()
                .Where(x => x.OntologyTerm.Value == ontologyTerm)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Collection>> ListByTitle(string title)
            => await _db.Collections
                .AsNoTracking()
                .Where(x => x.Title == title)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<bool> IsFromApi(int id)
            => await _db.Collections.AnyAsync(x => x.CollectionId == id && x.FromApi);

        /// <inheritdoc/>
        public async Task<bool> HasSampleSets(int id)
            => await _db.SampleSets.AnyAsync(x => x.CollectionId == id);

        /// <inheritdoc/>
        public async Task<Dictionary<int, string>> GetDescriptionsByCollectionIds(IEnumerable<int> collectionIds)
            => await _db.Collections
                .AsNoTracking()
                .Where(x => collectionIds.Contains(x.CollectionId))
                .Select(x => new
                {
                    id = x.CollectionId,
                    description = x.Description
                }).ToDictionaryAsync(x => x.id, x => x.description);

        /// <inheritdoc/>
        public async Task<Collection> GetCollectionByIdForIndexingAsync(int id)
            => (await _db.Collections
                    .AsNoTracking()
                    .Where(x => x.CollectionId == id)
                    .Include(x => x.OntologyTerm)
                    .Include(x => x.AccessCondition)
                    .Include(x => x.CollectionType)
                    .Include(x => x.CollectionStatus)
                    .Include(x => x.ConsentRestrictions)
                    .Include(x => x.AssociatedData)
                        .ThenInclude(x => x.AssociatedDataType)
                    .Include(x => x.AssociatedData)
                        .ThenInclude(x => x.AssociatedDataProcurementTimeframe)
                    .Include(x => x.SampleSets)
                    .FirstOrDefaultAsync()
                );
        
        /// <inheritdoc/>
        public async Task UpdateCollectionDetails(int collectionId)
        {
            // Get the collection out of the database.
            var collection = await GetCollectionByIdForIndexingAsync(collectionId);

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
    }
}
