using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Api.ReferenceData;

namespace Biobanks.SubmissionApi.Services.Contracts
{
    /// <summary>
    /// Service for handling Material Type Group reference data.
    /// </summary>
    public interface IMaterialTypeGroupService
    {
        /// <summary>
        /// Get a Material Type Group entity.
        /// </summary>
        /// <param name="materialTypeGroupId">The ID of the Material Type Group to fetch.</param>
        /// <returns>The requested Material Type Group.</returns>
        Task<MaterialTypeGroup> Get(int materialTypeGroupId);

        /// <summary>
        /// Get a list of Material Type Group entities.
        /// </summary>
        /// <returns>A list of Material Type Group entities.</returns>
        Task<IEnumerable<MaterialTypeGroup>> List();

        /// <summary>
        /// Inserts a new Material Type Group entity into the repository.
        /// </summary>
        /// <param name="materialTypeGroup">The Material Type Group object to insert.</param>
        /// <returns>The created Material Type Group.</returns>
        Task<MaterialTypeGroup> Create(MaterialTypeGroup materialTypeGroup);
        
        /// <summary>
        /// Updates an existing Material Type Group entity in the respository.
        /// </summary>
        /// <param name="materialTypeGroup">The Material Type Group object to update.</param>
        /// <returns></returns>
        Task Update(MaterialTypeGroup materialTypeGroup);

        /// <summary>
        /// Get a Material Type Group entity.
        /// </summary>
        /// <param name="value">The value of the Material Type Group to fetch.</param>
        /// <returns>The requested Material Type Group.</returns>
        Task<MaterialTypeGroup> GetByValue(string value);
    }
}