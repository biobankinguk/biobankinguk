using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Config
{
    /// <summary>
    /// Possible recurring jobs that the API can trigger using Hangfire.
    /// Opted into via configuration.
    /// </summary>
    public static class WorkersRecurringJobs
    {
        public const string Analytics = "analytics";
        public const string Publications = "publications";
        public const string Aggregator = "aggregator";
        public const string SubmissionsExpiry = "submissions-expiry";
    }

    /// <summary>
    /// Implementations the API can use for queueing background worker jobs
    /// </summary>
    public enum WorkersQueueService
    {
        AzureQueueStorage,
        Hangfire
    }

    /// <summary>
    /// Options for interacting with Workers,
    /// including which scheduled jobs the API should run using Hangfire,
    /// and how the API should queue its jobs
    /// </summary>
    public class WorkersOptions
    {
        public List<string> HangfireRecurringJobs { get; set; } = new();

        public WorkersQueueService QueueService { get; set; } = WorkersQueueService.Hangfire;
    }
}
