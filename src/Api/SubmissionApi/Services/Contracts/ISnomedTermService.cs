using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Api.ReferenceData;

namespace Biobanks.SubmissionApi.Services.Contracts
{
    /// <summary>
    /// Service for handling Snomed Term reference data.
    /// </summary>
    public interface ISnomedTermService
    {
        /// <summary>
        /// Get a Snomed Term entity.
        /// </summary>
        /// <param name="snomedTermId">The ID of the Snomed Term to fetch.</param>
        /// <returns>The requested Snomed Term.</returns>
        Task<SnomedTerm> Get(string snomedTermId);

        /// <summary>
        /// Get a list of Snomed Term entities.
        /// </summary>
        /// <returns>A list of Snomed Term entities.</returns>
        Task<IEnumerable<SnomedTerm>> List();

        /// <summary>
        /// Inserts a new Snomed Term entity into the repository.
        /// </summary>
        /// <param name="snomedTerm">The Snomed Term object to insert.</param>
        /// <returns>The created Snomed Term.</returns>
        Task<SnomedTerm> Create(SnomedTerm snomedTerm);
        
        /// <summary>
        /// Updates an existing Snomed Term entity in the respository.
        /// </summary>
        /// <param name="snomedTerm">The Snomed Term object to update.</param>
        /// <returns></returns>
        Task Update(SnomedTerm snomedTerm);

        /// <summary>
        /// Get a Snomed Term entity.
        /// </summary>
        /// <param name="value">The value of the Snomed Term to fetch.</param>
        /// <returns>The requested Snomed Term.</returns>
        Task<SnomedTerm> GetByValue(string value);

        /// <summary>
        /// Adds the givern Snomed Terms to the repository.
        /// </summary>
        /// <param name="snomedTerms">Snomed Terms to be added to the repository.</param>
        /// <returns></returns>
        Task Create(IList<SnomedTerm> snomedTerms);
        
        /// <summary>
        /// Updates the given Snomed Terms in the repository.
        /// </summary>
        /// <param name="snomedTerms">The Snomed Terms to update in the repository.</param>
        /// <returns></returns>
        Task Update(List<SnomedTerm> snomedTerms);
    }
}