using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Data;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts;

public interface ICapabilityService
{

    Task<IEnumerable<int>> GetAllCapabilityIdsAsync();
    Task<int> GetCapabilityCountAsync();
    Task<int> GetIndexableCapabilityCountAsync();

    Task<int> GetSuspendedCapabilityCountAsync();

    Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexingAsync(IEnumerable<int> capabilityIds);

    Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexDeletionAsync(IEnumerable<int> capabilityIds);
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DiagnosisCapability> GetCapabilityByIdForIndexingAsync(int id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ontologyTerm"></param>
    /// <returns></returns>
    Task<IEnumerable<int>> GetCapabilityIdsByOntologyTermAsync(string ontologyTerm);

    Task<DiagnosisCapability> GetCapabilityByIdAsync(int id);

    Task<IEnumerable<DiagnosisCapability>> ListCapabilitiesAsync(int organisationId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ontologyTerm"></param>
    /// <returns></returns>
    Task UpdateCapabilitiesOntologyOtherTerms(string ontologyTerm);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="capabilityId"></param>
    /// <returns></returns>
    Task UpdateCapabilityDetails(int capabilityId);
}