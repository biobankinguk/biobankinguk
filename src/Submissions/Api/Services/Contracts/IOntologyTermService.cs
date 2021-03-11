using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.SubmissionApi.Services.Contracts
{
    /// <summary>
    /// Service for handling Snomed Term reference data.
    /// </summary>
    public interface IOntologyTermService
    {
        /// <summary>
        /// Get a Snomed Term entity.
        /// </summary>
        /// <param name="ontologyTermId">The ID of the Snomed Term to fetch.</param>
        /// <returns>The requested Snomed Term.</returns>
        Task<OntologyTerm> Get(string ontologyTermId);

        /// <summary>
        /// Get a list of Snomed Term entities.
        /// </summary>
        /// <returns>A list of Snomed Term entities.</returns>
        Task<IEnumerable<OntologyTerm>> List();

        /// <summary>
        /// Inserts a new Snomed Term entity into the repository.
        /// </summary>
        /// <param name="ontologyTerm">The Snomed Term object to insert.</param>
        /// <returns>The created Snomed Term.</returns>
        Task<OntologyTerm> Create(OntologyTerm ontologyTerm);
        
        /// <summary>
        /// Updates an existing Snomed Term entity in the respository.
        /// </summary>
        /// <param name="ontologyTerm">The Snomed Term object to update.</param>
        /// <returns></returns>
        Task Update(OntologyTerm ontologyTerm);

        /// <summary>
        /// Get a Snomed Term entity.
        /// </summary>
        /// <param name="value">The value of the Snomed Term to fetch.</param>
        /// <returns>The requested Snomed Term.</returns>
        Task<OntologyTerm> GetByValue(string value);

        /// <summary>
        /// Adds the givern Snomed Terms to the repository.
        /// </summary>
        /// <param name="ontologyTerms">Snomed Terms to be added to the repository.</param>
        /// <returns></returns>
        Task Create(IList<OntologyTerm> ontologyTerms);
        
        /// <summary>
        /// Updates the given Snomed Terms in the repository.
        /// </summary>
        /// <param name="ontologyTerms">The Snomed Terms to update in the repository.</param>
        /// <returns></returns>
        Task Update(List<OntologyTerm> ontologyTerms);
    }
}