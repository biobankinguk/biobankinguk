using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Data;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts;

public interface ICapabilityService
{

    /// <summary>
    /// Get all Capability Ids
    /// </summary>
    /// <returns>A list of all Capability Ids</returns>
    Task<IEnumerable<int>> GetAllCapabilityIdsAsync();
    
    /// <summary>
    /// Get a count of all Capabilities
    /// </summary>
    /// <returns>The number of all Capabilities</returns>
    Task<int> GetCapabilityCountAsync();
    
    /// <summary>
    /// Get a count of all Capabilities that can be indexed
    /// Filters out suspended organisations.
    /// </summary>
    /// <returns>The number of indexable Capabilities.</returns>
    Task<int> GetIndexableCapabilityCountAsync();
    
    /// <summary>
    /// Get a count of Capabilitities where the organisation is suspended.
    /// </summary>
    /// <returns>The number of Capabilities.</returns>
    Task<int> GetSuspendedCapabilityCountAsync();

    /// <summary>
    /// Gets Capabilities that can be indexed.
    /// Filters out suspended organisations.
    /// </summary>
    /// <param name="capabilityIds">A list of capability Ids to check</param>
    /// <returns>A list of Capabilities.</returns>
    Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexingAsync(IEnumerable<int> capabilityIds);

    /// <summary>
    /// Gets Capabilities for deletion.
    /// </summary>
    /// <param name="capabilityIds">A list of capability Ids.</param>
    /// <returns>A list of Capabilities.</returns>
    Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexDeletionAsync(IEnumerable<int> capabilityIds);
    
    
    /// <summary>
    /// Gets a Capability for indexing..
    /// </summary>
    /// <param name="id">Id of the Capability to fetch.</param>
    /// <returns>The corresponding Capability.</returns>
    Task<DiagnosisCapability> GetCapabilityByIdForIndexingAsync(int id);

    /// <summary>
    /// Gets Capability Ids given an ontology term.
    /// </summary>
    /// <param name="ontologyTerm">The ontology term to match by.</param>
    /// <returns>A list of Capability Ids</returns>
    Task<IEnumerable<int>> GetCapabilityIdsByOntologyTermAsync(string ontologyTerm);

    /// <summary>
    /// Gets a Capability by its Id.
    /// </summary>
    /// <param name="id">Id of the Capability to match.</param>
    /// <returns>The correspodning Capability</returns>
    Task<DiagnosisCapability> GetCapabilityByIdAsync(int id);

    /// <summary>
    /// List all Capabilities of an organisation.
    /// </summary>
    /// <param name="organisationId">Id of the Organisation to match.</param>
    /// <returns>A list of Capabilities.</returns>
    Task<IEnumerable<DiagnosisCapability>> ListCapabilitiesAsync(int organisationId);

    /// <summary>
    /// Update all Capabilities with the given ontology term.
    /// </summary>
    /// <param name="ontologyTerm">Ontology term to update with.</param>
    Task UpdateCapabilitiesOntologyOtherTerms(string ontologyTerm);

    /// <summary>
    /// Update a Capability in the search index given its Id.
    /// </summary>
    /// <param name="capabilityId">The Id of the Capability to update.</param>
    Task UpdateCapabilityDetails(int capabilityId);
}
