using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Config
{
    /// <summary>
    /// Possible recurring jobs that the API can trigger using Hangfire.
    /// Opted into via configuration.
    /// </summary>
    public static class WorkersRecurringJobs
    {
        /// <summary>
        /// The Google Analytics data fetcher
        /// </summary>
        public const string Analytics = "analytics";

        /// <summary>
        /// The EPMC Publications data fetcher
        /// </summary>
        public const string Publications = "publications";

        /// <summary>
        /// The Submissions -> Directory Aggregator
        /// </summary>
        public const string Aggregator = "aggregator";

        /// <summary>
        /// The expired Submissions clean up
        /// </summary>
        public const string SubmissionsExpiry = "submissions-expiry";
    }

    /// <summary>
    /// Implementations the API can use for queueing background worker jobs
    /// </summary>
    public enum WorkersQueueService
    {
        /// <summary>
        /// Queued Worker Jobs (e.g. Submissions Staging) will be queued via Azure Queue Storage,
        /// which should in turn trigger some external worker run (e.g. Functions, WebJobs...)
        /// </summary>
        AzureQueueStorage,

        /// <summary>
        /// Queued Worker Jobs will be queued and triggered by Hangfire and run by this API process
        /// </summary>
        Hangfire
    }

    /// <summary>
    /// Options for interacting with Workers,
    /// including which scheduled jobs the API should run using Hangfire,
    /// and how the API should queue its jobs
    /// </summary>
    public class WorkersOptions
    {
        /// <summary>
        /// Which jobs as per <see cref="WorkersRecurringJobs" /> should be scheduled by this API's Hangfire server
        /// </summary>
        public List<string> HangfireRecurringJobs { get; set; } = new();

        /// <summary>
        /// Which service to use for Queued Worker Jobs
        /// </summary>
        public WorkersQueueService QueueService { get; set; } = WorkersQueueService.Hangfire;
    }
}
