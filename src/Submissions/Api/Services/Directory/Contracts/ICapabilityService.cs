using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Data;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts;

public interface ICapabilityService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ontologyTerm"></param>
    /// <returns></returns>
    Task<IEnumerable<int>> GetCapabilityIdsByOntologyTermAsync(string ontologyTerm);

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
    Task UpdateCapabilitiesOntologyOtherTerms(string ontologyTerm);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="capabilityId"></param>
    /// <returns></returns>
    Task UpdateCapabilityDetails(int capabilityId);
}