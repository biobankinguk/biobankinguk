using System;
using System.Threading.Tasks;

namespace Core.Submissions.Services.Contracts
{
    /// <summary>
    /// Service for writing objects to blob storage providers.
    /// </summary>
    public interface IBlobWriteService
    {
        /// <summary>
        /// Stores a generic object as its base type in a given blob storage container.
        /// </summary>
        /// <param name="container">ID of the container to store the object in.</param>
        /// <param name="obj">Object to store in the container.</param>
        /// <returns></returns>
        Task<Guid> StoreObjectAsJsonAsync(string container, object obj);

        /// <summary>
        /// Stores a text object in a given blob storage container with an associated MIME type.
        /// </summary>
        /// <param name="container">ID of the container to store the object in.</param>
        /// <param name="text">Text to store in the container.</param>
        /// <param name="contentType">MIME type of the text.</param>
        /// <returns></returns>
        Task<Guid> StoreTextAsync(string container, string text, string contentType = "text/plain");

        /// <summary>
        /// Delete a Blob
        /// </summary>
        /// <param name="container">ID of the container the Blob is stored in</param>
        /// <param name="id">ID of the Blob record to delete</param>
        /// <returns></returns>
        Task DeleteAsync(string container, Guid id);
    }
}
