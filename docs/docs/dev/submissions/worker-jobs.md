# Worker Jobs

A series of background tasks are processed via the Submissions API as worker jobs. These jobs are defined in the `lib/Core/Jobs` project folder. Each worker job is queued via the Directory's Hangfire instance.

You can opt in to which scheduled/recurring jobs will be run by your instance of the Directory using the following configuration:

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

A series of background tasks are processed via the Submissions API as worker jobs. These jobs are defined in the `lib/Core/Jobs` project folder.

## Aggregator Job

The `Aggregator Job` ingests committed Samples from the Submission API and generates Collections and SampleSets on the Directory. Samples flagged for deletion are also deleted and the changes propagated to the relevant Collections. The Aggregator job runs daily (00:00 UTC).

The Aggregator must be configured, such that it can aggregate samples into collections correctly. A standard configuration consists of

| Key | Value | Description |
| - | - | - |
| `Aggregator:NonExtractedOntologyTerm` |`string`| The default `OntologyTermId` to aggregate non-extracted samples under. |
| `Aggregator:MacroscopicAssessmentMappings` |`array` | An array of mappings between `ContentMethod`, `ContentId` => `MacroscopicAssessment` |

Both `NonExtractedOntologyTerm` and `MacroscopicAssessmentMappings` are required configuration options; with `MacroscopicAssessmentMappings` requiring at least one default mapping.

A default mapping is one where the (`ContentMethod`,`ContentId`) are both absent, and therefore applies to all samples. More specific mappings can be included, with strict matching to (`ContentMethod`,`ContentId`) and relaxed matching to just `ContentMethod`.

Mapping priority occurs with strictest matching first.

```json
// Strictest Mapping - First priority
{
  "ContentMethod": "Microscopic Assessment", // SampleContentMethod Of Sample
  "ContentId": "23875004",                   // SampleContentId Of Sample (OntologyTermId)
  "MacroscopicAssessment": "Non-Affected"    // MacroscopicAssessment to map to
},
// Midpoint - Will match with `ContentMethod` if no higher mapping rules apply
{
  "ContentMethod": "Macroscopic Assessment",
  "ContentId": "",
  "MacroscopicAssessment": "Affected"
},
// Default Mapping - Lowest priority
{
  "ContentMethod": "",
  "ContentId": "",
  "MacroscopicAssessment": "Not Applicable"
}
```

In the case of misconfigured mappings. The Aggregator will fail to run, logging the error to the Hangfire dashboard.

## Analytics Job

The `Analytics Job` fetches data for later report generation using Google Analytics Data that is associated with a Directory instance. Reports can be ran for the entirety of the Directory, or for each registered organisation.

By default, the reports are generated quarterly.

The Worker Job itself simply fetches data from the Google Analytics Reporting API for a given view for a given Directory instance. It transforms and stores that data locally in the Directory's database.

The subsequent report generation occurs on demand when the Directory calls one of 2 API endpoints which transform the locally stored data (put there by the Worker Job run) into a report dataset.

### Google Analytics Authentication

For authentication with the Google Analytics Reporting API, a Service Account key file (JSON) is necessary in order to work with OAuth2.

More details on that here: https://developers.google.com/analytics/devguides/reporting/core/v4/authorization

### Configuration

| Key | Value | Description |
| - | - | - |
| `Analytics:GoogleAnalyticsReportingKey` | `JSON` | Json object in Google Analytics Reporting key file, see above |
| `Analytics:GoogleAnalyticsViewId` | `int` | ID of the Google Analytics view |
| `Analytics:StartDate` | `"yyyy-MM-dd"` | Start date of the timeframe to download Google Analytics data (default: `2016-01-01`) |

## Publications Job

The `Publication Job` is an aggregator that queries the [Europe PubMed Central API](https://europepmc.org/RestfulWebService) to find potential articles associated with organisations that are registered on the Directory.

Due to the volume of articles hosted by EPMC, these articles and their relevant metadata are cached in the working database, so that it can be accessed in real-time by users using the Directory. 

The `Publication Job` is a scheduled job that runs daily (00:00 UTC). Individual organisations are able to opt-out of this feature via the ADAC panel on the Directory.

### Configuration

| Key | Value | Description |
| - | - | - |
| `Publications:EpmcApiUrl` | `https://www.ebi.ac.uk/europepmc/` | The base URL of the EMPC API, this should only change when the API updates |
