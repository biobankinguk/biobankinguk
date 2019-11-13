using System.Threading.Tasks;

namespace Upload.Contracts
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
    }
}
