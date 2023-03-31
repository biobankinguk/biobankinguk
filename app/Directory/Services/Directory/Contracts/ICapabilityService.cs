using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Directory.Models.Directory;

namespace Biobanks.Directory.Services.Directory.Contracts;

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

    /// <summary>
    /// Adds a Capability 
    /// </summary>
    /// <param name="capabilityDTO">The DTO of the Capability to Add.</param>
    /// <param name="associatedData">The associated Data add.</param>
    Task AddCapabilityAsync(CapabilityDTO capabilityDTO, IEnumerable<CapabilityAssociatedData> associatedData);

    /// <summary>
    /// Update a Capability 
    /// </summary>
    /// <param name="capabilityDTO">The Id of the Capability to update.</param>
    /// <param name="associatedData">The associated Data add.</param>
    Task UpdateCapabilityAsync(CapabilityDTO capabilityDTO, IEnumerable<CapabilityAssociatedData> associatedData);

    /// <summary>
    /// Delete a Capability 
    /// </summary>
    /// <param name="id">The Id of the Capability to delete.</param>
    Task DeleteCapabilityAsync(int id);
}
