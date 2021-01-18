using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Api.ReferenceData;

namespace Biobanks.SubmissionApi.Services.Contracts
{
    /// <summary>
    /// Service for handling Ontology Version reference data.
    /// </summary>
    public interface IOntologyVersionService
    {
        /// <summary>
        /// Get an Ontology Version entity.
        /// </summary>
        /// <param name="ontologyVersionId">The ID of the Ontology Version to fetch.</param>
        /// <returns>The requested Ontology Version.</returns>
        Task<OntologyVersion> Get(int ontologyVersionId);

        /// <summary>
        /// Get a list of Ontology Version entities.
        /// </summary>
        /// <param name="ontologyId">ID of the Ontology to retrieve versions for.</param>
        /// <returns>A list of Ontology Version entities.</returns>
        Task<IEnumerable<OntologyVersion>> List(int? ontologyId);

        /// <summary>
        /// Inserts a new Ontology Version entity into the repository.
        /// </summary>
        /// <param name="ontologyVersion">The Ontology Version object to insert.</param>
        /// <returns>The created Ontology Version.</returns>
        Task<OntologyVersion> Create(OntologyVersion ontologyVersion);
        
        /// <summary>
        /// Updates an existing Ontology Version entity in the respository.
        /// </summary>
        /// <param name="ontologyVersion">The Ontology Version object to update.</param>
        /// <returns></returns>
        Task Update(OntologyVersion ontologyVersion);

        /// <summary>
        /// Get a Ontology Version entity.
        /// </summary>
        /// <param name="value">The value of the Ontology Version to fetch.</param>
        /// <returns>The requested Ontology Version.</returns>
        Task<OntologyVersion> GetByValue(string value);
    }
}