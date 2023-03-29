using System.Threading.Tasks;

namespace Core.Submissions.Services.Contracts
{
    /// <summary>
    /// Service for writing messages to storage queues.
    /// </summary>
    public interface IQueueWriteService
    {
        /// <summary>
        /// Pushes a new message to a given storage queue.
        /// </summary>
        /// <param name="queue">ID of the storage queue to push the message to.</param>
        /// <param name="message">Message to push to the given storage queue.</param>
        /// <returns></returns>
        Task PushAsync(string queue, string message);
    
        /// <summary>
        /// Delete a message from a storage queue
        /// </summary>
        /// <param name="queue">ID of the storage queue containing the message.</param>
        /// <param name="messageId">ID of the message to delete.</param>
        /// <param name="popReceipt">Unique ID issued when the message was popped, so only the popper can delete.</param>
        /// <returns></returns>
        Task DeleteAsync(string queue, string messageId, string popReceipt);
    }
}