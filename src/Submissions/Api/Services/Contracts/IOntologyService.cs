using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Api.ReferenceData;

namespace Biobanks.SubmissionApi.Services.Contracts
{
    /// <summary>
    /// Service for handling Ontology reference data.
    /// </summary>
    public interface IOntologyService
    {
        /// <summary>
        /// Get an Ontology entity.
        /// </summary>
        /// <param name="ontologyId">The ID of the Ontology to fetch.</param>
        /// <returns>The requested Ontology.</returns>
        Task<Ontology> Get(int ontologyId);

        /// <summary>
        /// Get a list of Ontology entities.
        /// </summary>
        /// <returns>A list of Ontology entities.</returns>
        Task<IEnumerable<Ontology>> List();

        /// <summary>
        /// Inserts a new Ontology entity into the repository.
        /// </summary>
        /// <param name="ontology">The Ontology object to insert.</param>
        /// <returns>The created Ontology.</returns>
        Task<Ontology> Create(Ontology ontology);
        
        /// <summary>
        /// Updates an existing Ontology entity in the respository.
        /// </summary>
        /// <param name="ontology">The Ontology object to update.</param>
        /// <returns></returns>
        Task Update(Ontology ontology);

        /// <summary>
        /// Get a Ontology entity.
        /// </summary>
        /// <param name="value">The value of the Ontology to fetch.</param>
        /// <returns>The requested Ontology.</returns>
        Task<Ontology> GetByValue(string value);
    }
}