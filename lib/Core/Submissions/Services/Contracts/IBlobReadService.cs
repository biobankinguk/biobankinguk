using System;
using System.Threading.Tasks;

namespace Core.Submissions.Services.Contracts
{
    /// <summary>
    /// Service for reading from blob storage providers.
    /// </summary>
    public interface IBlobReadService
    {
        /// <summary>
        /// Rturns a string object from a given storage container from its unique blob identifier.
        /// </summary>
        /// <param name="container">ID of the blob storage container to read from.</param>
        /// <param name="id">ID of the blob to retrieve from the storage container.</param>
        /// <returns></returns>
        Task<string> GetObjectFromJsonAsync(string container, Guid id);
    }
}
