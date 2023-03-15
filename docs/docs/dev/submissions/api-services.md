# API Services

## Submissions Service

The Submissions Service is an optional additional service which allows for low-level (sample level data) bulk submissions to the Central Tissue Directory Database. These submissions will ultimately be aggregated into searchable Directory Collections.

The Submissions Service consists of the following elements:

- An HTTP API for accepting and managing data in bulk
- Two Worker processes:
  - A scheduled Expiry task which runs on a schedule and expires stale uncommitted submissions
  - A background Processing task which performs validation and persistence of accepted submissions
    - this is not performed by the API directly due to the time it can take to process large submissions
