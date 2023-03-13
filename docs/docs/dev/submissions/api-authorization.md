# API Authorization

Requests to the API must be authenticated and authorized.

## Token Auth

Endpoints intended for public consumption are token authenticated. The following describes

1. First an API Client must exchange valid credentials with the `/token` endpoint, to get an access token.
1. The token can be used with interactions on all other endpoints until it expires (1 day after issue currently)
    - in the event a token has expired, a new one may be acquired provided credentials are still valid.

## Basic Auth

Internally used endpoints (e.g. for the Directory web app to hit) usually just accept the Directory's Client Credentials over Basic Auth, the same mechanism as for requesting a token.

## Client Credentials

Valid Client Credentials means a matching record in the `ApiClients` table for a Client ID and a hashed Client Secret.

Ideally, API Clients are added to Organisations through the Directory Frontend.
If that is not an option, it is recommended to use the **IdentityTool** in this repo to add clients, or at least to generate credentials:

1. `cd /src/IdentityModel/IdentityTool`
1. `dotnet run -- api-clients add --generate --connection-string "<connection-string>" <biobankId>`

ApiClients are usually associated with Biobanks and the API verifies that the access token in use is authorised to act on a given Biobank's data.

The **IdentityTool** is the only way to add credentials that aren't Biobank affiliated, for use at a system level (e.g. by the Directory web app).
