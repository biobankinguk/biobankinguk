# Directory & Submissions

## API Services

### Analytics Reports

The Directory web app calls on endpoints in this API to generate reports data from locally cached Google Analytics data.

Some configuration is required:

| Key | Value | Description |
|-|-|-|
| `Analytics:FilterHostname` | `string` | An optional hostname to filter the analytics data on; if none is provided, no filtering will occur. |
| `Analytics:MetricThreshold` | `int` | Number of Organisations to include in the ranking for Organisation reports (default: `10`) |
| `Analytics:EventThreshold` | `int` | Number of Event groups that originated from the same location on a particular day above which will be excluded from plots (default: `30`) |

### Submissions Service

The Submissions Service is an optional additional service which allows for low-level (sample level data) bulk submissions to the Central Tissue Directory Database. These submissions will ultimately be aggregated into searchable Directory Collections.

The Submissions Service consists of the following elements:

- An HTTP API for accepting and managing data in bulk
- Two Worker processes:
  - A scheduled Expiry task which runs on a schedule and expires stale uncommitted submissions
  - A background Processing task which performs validation and persistence of accepted submissions
    - this is not performed by the API directly due to the time it can take to process large submissions

## API Authorization

Requests to the API must be authenticated and authorized.

### Token Auth

Endpoints intended for public consumption are token authenticated. The following describes

1. First an API Client must exchange valid credentials with the `/token` endpoint, to get an access token.
1. The token can be used with interactions on all other endpoints until it expires (1 day after issue currently)
    - in the event a token has expired, a new one may be acquired provided credentials are still valid.

### Basic Auth

Internally used endpoints (e.g. for the Directory web app to hit) usually just accept the Directory's Client Credentials over Basic Auth, the same mechanism as for requesting a token.

### Client Credentials

Valid Client Credentials means a matching record in the `ApiClients` table for a Client ID and a hashed Client Secret.

Ideally, API Clients are added to Organisations through the Directory Frontend.
If that is not an option, it is recommended to use the **IdentityTool** in this repo to add clients, or at least to generate credentials:

1. `cd /src/IdentityModel/IdentityTool`
1. `dotnet run -- api-clients add --generate --connection-string "<connection-string>" <biobankId>`

ApiClients are usually associated with Biobanks and the API verifies that the access token in use is authorised to act on a given Biobank's data.

The **IdentityTool** is the only way to add credentials that aren't Biobank affiliated, for use at a system level (e.g. by the Directory web app).

## Worker Jobs

A series of background tasks are processed via the Submissions API as worker jobs. These jobs are defined in the `src/Core/Jobs` project folder. Each worker job is queued via the API's Hangfire instance, which uses the `apiHangfire` schema.

You can opt in to which scheduled/recurring jobs will be run by your instance of the API usnig the following configuration:

```jsonc
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
