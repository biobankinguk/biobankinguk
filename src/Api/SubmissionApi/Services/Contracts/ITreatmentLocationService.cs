using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Api.ReferenceData;

namespace Biobanks.SubmissionApi.Services.Contracts
{
    /// <summary>
    /// Service for handling Treatment Location reference data.
    /// </summary>
    public interface ITreatmentLocationService
    {
        /// <summary>
        /// Get a Treatment Location entity.
        /// </summary>
        /// <param name="treatmentLocationId">The ID of the Treatment Location to fetch.</param>
        /// <returns>The requested Treatment Location.</returns>
        Task<TreatmentLocation> Get(int treatmentLocationId);

        /// <summary>
        /// Get a list of Treatment Location entities.
        /// </summary>
        /// <returns>A list of Treatment Location entities.</returns>
        Task<IEnumerable<TreatmentLocation>> List();

        /// <summary>
        /// Inserts a new Treatment Location entity into the repository.
        /// </summary>
        /// <param name="treatmentLocation">The Treatment Location object to insert.</param>
        /// <returns>The created Treatment Location.</returns>
        Task<TreatmentLocation> Create(TreatmentLocation treatmentLocation);
        
        /// <summary>
        /// Updates an existing Treatment Location entity in the respository.
        /// </summary>
        /// <param name="treatmentLocation">The Treatment Location object to update.</param>
        /// <returns></returns>
        Task Update(TreatmentLocation treatmentLocation);

        /// <summary>
        /// Get a Treatment Location entity.
        /// </summary>
        /// <param name="value">The value of the Treatment Location to fetch.</param>
        /// <returns>The requested Treatment Location.</returns>
        Task<TreatmentLocation> GetByValue(string value);
    }
}