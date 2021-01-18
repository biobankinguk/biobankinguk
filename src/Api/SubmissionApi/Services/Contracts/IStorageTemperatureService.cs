using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Shared.ReferenceData;

namespace Biobanks.SubmissionApi.Services.Contracts
{
    /// <summary>
    /// Service for handling Storage Temperature reference data.
    /// </summary>
    public interface IStorageTemperatureService
    {
        /// <summary>
        /// Get a Storage Temperature entity.
        /// </summary>
        /// <param name="storageTemperatureId">The ID of the Storage Temperature to fetch.</param>
        /// <returns>The requested Storage Temperature.</returns>
        Task<StorageTemperature> Get(int storageTemperatureId);

        /// <summary>
        /// Get a list of Storage Temperature entities.
        /// </summary>
        /// <returns>A list of Storage Temperature entities.</returns>
        Task<IEnumerable<StorageTemperature>> List();

        /// <summary>
        /// Inserts a new Storage Temperature entity into the repository.
        /// </summary>
        /// <param name="storageTemperature">The Storage Temperature object to insert.</param>
        /// <returns>The created STorage Temperature.</returns>
        Task<StorageTemperature> Create(StorageTemperature storageTemperature);
        
        /// <summary>
        /// Updates an existing Storage Temperature entity in the respository.
        /// </summary>
        /// <param name="storageTemperature">The Storage Temperature object to update.</param>
        /// <returns></returns>
        Task Update(StorageTemperature storageTemperature);

        /// <summary>
        /// Get a Storage Temperature entity.
        /// </summary>
        /// <param name="value">The value of the Storage Temperature to fetch.</param>
        /// <returns>The requested Storage Temperature.</returns>
        Task<StorageTemperature> GetByValue(string value);
    }
}