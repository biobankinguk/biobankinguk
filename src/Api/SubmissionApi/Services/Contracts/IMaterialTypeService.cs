using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Shared.ReferenceData;
using Biobanks.Common.Models;

namespace Biobanks.SubmissionApi.Services.Contracts
{
    /// <summary>
    /// Service for handling Material Type reference data.
    /// </summary>
    public interface IMaterialTypeService
    {
        /// <summary>
        /// Get a Material Type entity.
        /// </summary>
        /// <param name="materialTypeId">The ID of the Material Type to fetch.</param>
        /// <returns>The requested Material Type.</returns>
        Task<MaterialType> Get(int materialTypeId);

        /// <summary>
        /// Get a list of Material Type entities.
        /// </summary>
        /// <returns>A list of Material Type entities.</returns>
        Task<IEnumerable<MaterialType>> List();

        /// <summary>
        /// Inserts a new Material Type entity into the repository.
        /// </summary>
        /// <param name="materialType">The Material Type object to insert.</param>
        /// <returns>The created Material Type.</returns>
        Task<MaterialType> Create(MaterialType materialType);
        
        /// <summary>
        /// Updates an existing Material Type entity in the respository.
        /// </summary>
        /// <param name="materialType">The Material Type object to update.</param>
        /// <returns></returns>
        Task Update(MaterialType materialType);

        /// <summary>
        /// Get a Material Type entity.
        /// </summary>
        /// <param name="value">The value of the Material Type to fetch.</param>
        /// <returns>The requested Material Type.</returns>
        Task<MaterialType> GetByValue(string value);

        /// <summary>
        /// Adds a collection of Material Types to the repository.
        /// </summary>
        /// <param name="materialTypes">Material Types to be added to the repository.</param>
        /// <returns></returns>
        Task Create(IList<MaterialTypeJsonModel> materialTypes);
    }
}