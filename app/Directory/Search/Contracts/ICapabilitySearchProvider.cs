using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Search.Dto.Facets;
using Biobanks.Search.Dto.Results;

namespace Biobanks.Search.Contracts
{
    /// <summary>
    /// Search (Read) methods against an index of Capabilities
    /// </summary>
    public interface ICapabilitySearchProvider
    {
        /// <summary>
        /// Search for capability documents for a given ontology term, with specified facets
        /// and a limit on results
        /// </summary>
        /// <param name="ontologyTerm">The ontologyTerm searched for</param>
        /// <param name="selectedFacets">Facets</param>
        /// <param name="maxHits">Results limit</param>
        /// <returns>a Result</returns>
        Result Search(string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets, int maxHits);

        /// <summary>
        /// Search for capability documents in a given Organisation,
        /// for a given ontology term, with specified facets and a limit on results
        /// </summary>
        /// <param name="organisationExternalId"></param>
        /// <param name="ontologyTerm"></param>
        /// <param name="selectedFacets"></param>
        /// <returns>a Result specifically for Organisation Capabilities</returns>
        BiobankCapabilityResult Search(string organisationExternalId, string ontologyTerm, IEnumerable<SelectedFacet> selectedFacets);

        /// <summary>
        /// Count of documents in the index
        /// </summary>
        /// <returns>The count</returns>
        Task<long> Count();

        /// <summary>
        /// A list of ALL document IDs in the index
        /// TODO this may be redundant now
        /// </summary>
        /// <returns>A list of document IDs</returns>
        Task<List<int>> ListIds();

        /// <summary>
        /// A list of Ontology Terms in this index, filtered by an optional wildcard match
        /// </summary>
        /// <param name="wildcard">The wildcard string to match</param>
        /// <returns>A list of matching Ontology Terms</returns>
        IEnumerable<OntologyTermsSummary> ListOntologyTerms(string wildcard = "");
    }
}