﻿using Biobanks.Entities.Shared.ReferenceData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface IOntologyTermService
    {
        /// <summary>
        /// Get all untracked OntologyTerms which match the given parameter filters
        /// </summary>
        /// <param name="value">The descriptive value of the OntologyTerm</param>
        /// <param name="tags"></param>
        /// <param name="onlyDisplayable">Whether to return only OntologyTerms that are displayable in the Directory</param>
        /// <returns>An Enumerable of all applicable untracked OntologyTerms</returns>
        Task<IEnumerable<OntologyTerm>> List(string value = null, List<string> tags = null, bool onlyDisplayable = false);

        /// <summary>
        /// Get all untracked OntologyTerms which match the given parameter filters
        /// </summary>
        /// <param name="skip">How many OntologyTerms to skip from the results</param>
        /// <param name="take">How many OntologyTerms to take from the results</param>
        /// <param name="value">The descriptive value of the OntologyTerm</param>
        /// <param name="tags">
        /// The tags to filter by. A null value will not filter by tag, where an empty list will only return
        /// untagged OntologyTerms. By default the value is null, hence tags as a filter will be ignored.
        /// </param>
        /// <param name="onlyDisplayable">Whether to return only OntologyTerms that are displayable in the Directory</param>
        /// <returns>An Enumerable of OntologyTerms of maximum length 'take', after skipping 'skip' OntologyTerms</returns>
        Task<IEnumerable<OntologyTerm>> ListPaginated(int skip, int take, string value = null, List<string> tags = null, bool onlyDisplayable = false);

        /// <summary>
        /// Delete a given OntologyTerm with Id
        /// </summary>
        /// <param name="id">Id of the OntologyTerm to delete. Assumed the OntologyTerm Id exists</param>
        Task Delete(string id);

        /// <summary>
        /// Returns singular OntologyTerm that matches the given parameter filters. If more than one OntologyTerm is found
        /// that matches the given filters, then the first result is given.
        /// </summary>
        /// <param name="id">The alphanumeric Id of the OntologyTerm</param>
        /// <param name="value">The descriptive value of the OntologyTerm</param>
        /// <param name="tags">
        /// The tags to filter by. A null value will not filter by tag, where an empty list will only return
        /// untagged OntologyTerms. By default the value is null, hence tags as a filter will be ignored.
        /// </param>
        /// <param name="onlyDisplayable">Whether to return only OntologyTerms that are displayable in the Directory</param>
        /// <returns>The first OntologyTerm that matches all given parameter filters</returns>
        Task<OntologyTerm> Get(string id = null, string value = null, List<string> tags = null, bool onlyDisplayable = false);

        /// <summary>
        /// Update an exisiting OntologyTerm with the provided updated entity. The method call should update both database and elastic
        /// search reference.
        /// </summary>
        /// <param name="ontologyTerm">The version of the OntologyTerm to update with</param>
        /// <returns>The updated OntologyTerm</returns>
        Task<OntologyTerm> Update(OntologyTerm ontologyTerm);

        /// <summary>
        /// Create a new OntologyTerm. The Id of the OntologyTerm should be null, as it is
        /// generated by the database
        /// </summary>
        /// <param name="ontologyTerm">The OntologyTerm to be created.</param>
        /// <returns>The newly created OntologyTerm, with assigned Id</returns>
        Task<OntologyTerm> Create(OntologyTerm ontologyTerm);

        /// <summary>
        /// Checks if an OntologyTerm exists with the given parameter filters.
        /// </summary>
        /// <param name="id">The alphanumeric Id of the OntologyTerm</param>
        /// <param name="value">The descriptive value of the OntologyTerm</param>
        /// <param name="tags">
        /// The tags to filter by. A null value will not filter by tag, where an empty list will only return
        /// untagged OntologyTerms. By default the value is null, hence tags as a filter will be ignored.
        /// </param>
        /// <returns>true - If at least one OntologyTerm exists that matches the given parameter filters</returns>
        Task<bool> Exists(string id = null, string value = null, List<string> tags = null);

        /// <summary>
        /// Check wheher the given OntologyTerm is being used by either a Collection or Capability 
        /// </summary>
        /// <param name="id">The alphanumeric Id of the OntologyTerm</param>
        Task<bool> IsInUse(string id);

        /// <summary>
        /// Counts the number of OntologyTerms which match the given parameter filters
        /// </summary>
        /// <param name="value">The descriptive value of the OntologyTerm</param>
        /// <param name="tags">
        /// The tags to filter by. A null value will not filter by tag, where an empty list will only return
        /// untagged OntologyTerms. By default the value is null, hence tags as a filter will be ignored.
        /// </param>
        /// <returns>The integer number of OntologyTerms which match the given parameter filters</returns>
        Task<int> Count(string value = null, List<string> tags = null);

        /// <summary>
        /// Counts how many occurances of the OntologyTerm in both Collections and Capabilities 
        /// </summary>
        /// <param name="ontologyTermId">The alphanumeric Id of the OntologyTerm</param>
        /// <returns>The integer number of occurances of the OntologyTerm in both Collections and Capabilities </returns>
        Task<int> CountCollectionCapabilityUsage(string ontologyTermId);
    }
}
