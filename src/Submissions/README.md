# Submissions Service

The Submissions Service is an optional additional service which allows for low-level (sample level data) bulk submissions to the Central Tissue Directory Database. These submissions will ultimately be aggregated into searchable Directory Collections.

The Submissions Service consists of the following elements:

- An HTTP API for accepting and managing data in bulk
- An Authentication service and token provider used to authorise use of the API (to be deprecated)
- Two Worker processes:
    - A scheduled Expiry task which runs on a schedule and expires stale uncommitted submissions
    - A background Processing task which performs validation and persistence of accepted submissions
        - this is not performed by the API directly due to the time it can take to process large submissions

Currently the Workers are available for Azure Functions v3, but other implementations may be considered.

# API Authorization

Requests to the Submissions API must be authenticated and authorized.

1. First an API Client must exchange valid credentials with the `/token` endpoint, to get an access token.
1. The token can be used with interactions on all other endpoints until it expires (1 day after issue currently)
    - in the event a token has expired, a new one may be acquired provided credentials are still valid.

Valid Client Credentials means a matching record in the `ApiClients` table for a Client ID and a hashed Client Secret.

Ideally, API Clients are added to Organisations through the Directory Frontend.
If that is not an option, it is recommended to use the **IdentityTool** in this repo to add clients, or at least to generate credentials:

1. `cd /src/IdentityModel/IdentityTool`
1. `dotnet run -- api-clients add --generate --connection-string "<connection-string>" <biobankId>`

ApiClients are associated with Biobanks and the API verifies that the access token in use is authorised to act on a given Biobank's data.