using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data.Entities;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface ICollectionService
    {
        /// <summary>
        /// Deletes an empty Collection. If the Collection has any SampleSets it will fail to be deleted
        /// </summary>
        /// <param name="id">The Id of the Collection to delete</param>
        Task<bool> Delete(int id);

        /// <summary>
        /// Create a new Collection. At the point of creation, the Collection's timestamp will be updated to reflect
        /// the latest change
        /// </summary>
        /// <returns>The newly created Collection with assigned idenitity Id</returns>
        Task<Collection> Add(Collection collection);


        /// <summary>
        /// Copy an exisiting Collection on both database and search index
        /// </summary>
        /// <returns>The copied Collection with updated fields and timestamp</returns>
        Task<Collection> Copy(int id, int biobankId);

        /// <summary>
        /// Update an exisiting Collection on both database and search index
        /// </summary>
        /// <returns>The updated Collection with updated fields and timestamp</returns>
        Task<Collection> Update(Collection collection);

        /// <summary>
        /// Get the untracked Collection with associated Collection Id
        /// </summary>
        /// <returns>
        /// <para>The Collection including populated fields:</para>
        /// <para>AccessCondition, AssociatedData, CollectionType, CollectionStatus, ConsentRestrictions and OntologyTerm</para>
        /// <para>Note: AssociatedData does NOT include it's child entities</para>
        /// </returns>
        Task<Collection> Get(int id);

        /// <summary>
        /// Get the untracked Collection with associated Collection Id, including all SampleSets and MaterialDetails
        /// </summary>
        /// <returns>
        /// <para>The Collection including all fields</para>
        /// </returns>
        Task<Collection> GetWithSampleSets(int id);

        /// <summary>
        /// Get the untracked Collection with associated Collection Id for use in search indexing
        /// </summary>
        /// <returns>
        /// <para>The Collection including populated fields for indexing:</para>
        /// <para>AccessCondition, AssociatedData, CollectionType, CollectionStatus, ConsentRestrictions, OntologyTerm and SampleSets</para>
        /// <para>Note: AssociatedData does include it's child entities</para>
        /// </returns>
        Task<Collection> GetForIndexing(int id);

        /// <summary>
        /// List untracked Collections
        /// </summary>
        /// <param name="organisationId">Optional filter by Organistation Id</param>
        /// <returns>Enumerable of untracked Collections</returns>
        Task<IEnumerable<Collection>> List(int organisationId = default);

        /// <summary>
        /// List untracked Collections, with given OntologyTerm value.
        /// </summary>
        /// <param name="ontologyTerm">Value of the OntologyTerm to filter by</param>
        /// <returns>Enumerable of untracked Collections</returns>
        Task<IEnumerable<Collection>> ListByOntologyTerm(string ontologyTerm);

        /// <summary>
        /// Whether the Collection has been created from aggreagted data submitted via the API.
        /// </summary>
        /// <param name="id">The Id of the Collecton</param>
        /// <returns>
        /// <list>
        ///     <item>
        ///         <term>true</term>
        ///         <description>If the Collection has been generated from aggregated data, submitted via the API</description>
        ///     </item>
        ///     <item>
        ///         <term>false</term>
        ///         <description>If the Collection was created by a user via the Directory</description>
        ///     </item>
        /// </list>
        /// </returns>
        Task<bool> IsFromApi(int id);

        /// <summary>
        /// Check if the Collection has associated SampleSets
        /// </summary>
        /// <param name="id">The Id of the Collection</param>
        Task<bool> HasSampleSets(int id);

        /// <summary>
        /// Gets descriptions for a list of collection ids
        /// </summary>
        /// <param name="collectionIds">List of collection Ids to get descriptions for.</param>
        /// <returns>A dictionary of collection ids and their descriptions.</returns>
        Task<Dictionary<int, string>> GetDescriptionsByCollectionIds(IEnumerable<int> collectionIds);

        /// <summary>
        /// Get the Collection with associated Collection Id for use in search indexing
        /// </summary>
        /// <param name="id">Id of the Collection</param>
        /// <returns>The collection with populated fields.</returns>
        Task<Collection> GetCollectionByIdForIndexingAsync(int id);

        /// <summary>
        /// Updates details of one Collection 
        /// </summary>
        /// <param name="collectionId">Id of the collection to update.</param>
        /// <returns></returns>
        Task UpdateCollectionDetails(int collectionId);

    }
}
