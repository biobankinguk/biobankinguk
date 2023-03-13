# Worker Jobs

A series of background tasks are processed via the Submissions API as worker jobs. These jobs are defined in the `src/Core/Jobs` project folder. Each worker job is queued via the API's Hangfire instance, which uses the `apiHangfire` schema.

You can opt in to which scheduled/recurring jobs will be run by your instance of the API usnig the following configuration:

```json
"Workers": {
    "HangfireRecurringJobs": [
      "aggregator", // The Submissions -> Directory Aggregator, runs daily
      "submissions-expiry", // The expired Submissions clean up, runs daily
      "analytics", // The Google Analytics data fetcher, runs quarterly
      "publications", // The EPMC Publications data fetcher, runs daily
    ]
  }
```

If you're using environment variables (or e.g. the Azure Portal), guidance on setting Config Array values is available [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#naming-of-environment-variables).

By default, no recurring jobs are enabled - only those you include in the above config array will run.

Documentation for each worker job can be found on the [Wiki - Worker Jobs](https://github.com/biobankinguk/biobankinguk/wiki/Worker-Jobs).

In future other implementations of these worker jobs may be possible.
