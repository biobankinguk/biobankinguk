using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface IPublicationService
    {
        /// <summary>
        /// Get all untracked Publication assoicated to a given Organisation
        /// </summary>
        /// <param name="acceptedOnly">Whether to return Publications that have been accepted by the Organisation, false by default</param>
        /// <returns>An Enumerable of all applicable untracked Publications</returns>
        Task<IEnumerable<Publication>> ListByOrganisation(int organisationId, bool acceptedOnly = false);

        /// <summary>
        /// Create a new Publication
        /// </summary>
        /// <returns>The newly created Publication with assigned Id</returns>
        Task<Publication> Create(Publication publication);

        /// <summary>
        /// Applies an update action to an exisiting Publication
        /// </summary>
        /// <param name="publicationId">The Id of the Publication to apply updates to</param>
        /// <param name="organisationId">The Id of the Organisation to which the Publication is associated with, as duplicate Publications may exist</param>
        /// <param name="updates">The update action which acts on a scoped, tracked Publication entity</param>
        /// <returns>An untracked, updated Publication entity. Otherwise null if Publication does not exist</returns>
        Task<Publication> Update(string publicationId, int organisationId, Action<Publication> updates);
    }
}
