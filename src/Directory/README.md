This contains the new Directory App.

Currently referred to as "Core Services", it provides core services used by the current Directory, and other projects such as the Upload API.

Services provided by the App at this time:

- Authentication
- Reference Data CRUD Endpoints

# Getting Started

#### Prerequisites:

1. .NET Core 3.x
2. node.js 5.x+
3. a SQL Server instance (e.g. LocalDB)

#### First time setup steps:

1. Initialise the database
2. Set Environment Configuration Settings

Details for each step follow below.

## 1. Initialise the database

The App uses Entity Framework Core with Code First Migrations.

Migrations do run automatically during App Startup, so you shouldn't normally need to update the database.

Because of the solution structure (featuring the Data bits in a Common class library), the right projects must be specified when adding migrations or applying them to a database:

### In Visual Studio Package Manager Console:

- Make sure to target the `Common` project.
- Have `Directory` set as **Startup Project**
- `Add-Migration <name>` or `Update-Database`

### Using `dotnet-ef` in the dotnet cli

- In the same directory as `Common.csproj` (which contains the `DbContext`)
- Specify the path to `Directory.csproj` as the Startup Project using the `-s` flag, e.g. :
- `dotnet ef migrations add <name> -s ../../Directory/Directory/Directory.csproj`
- `dotnet ef database update -s ../../Directory/Directory/Directory.csproj`

## 2. Set Environment Configuration Settings

There are a few configuration settings that are not in source control because they are considered secrets, or expected to change.

There are two ways these can be configured:

### Using Environment Variables

In ASP.NET Core applications, any configuration value can be set on the environment by setting an Environment Variable named as follows:

`DOTNET_<setting key>` or `ASPNETCORE_<setting key>`, where `<setting key>` is replaced by the name of the relevant configuration setting.

### Using Visual Studio

Visual Studio provides a means to set values which should remain outside source control via User Secrets.

Simply right-click the **Directory** Project and choose "Manage User Secrets..." to add the secrets to a json file, similar to the source controlled `appsettings.json`.

### Settings

The settings which need configuring are as follows:

| Setting Key | Description | Example |
| - | - | - |
| `SuperAdminSeedPassword` | A password used to seed the SuperAdmin user on first run. **Must not** be empty. | `test` |
| `TrustedClients:upload-api:secret` | The Client Secret for the Upload API Client. | `test` |
| `OutboundEmail:SendGridApiKey` | An API key for SendGrid. If populated, emails will be sent via SendGrid instead of the local debug service. | `<a SendGrid API key>` |

# Architecture Overview

In some ways this application is two (or three) apps.

- The ASP.NET Core app itself is primarily a headless backend
  - providing API endpoints, mainly for interaction with its backing database.
- Minimal ASP.NET Core frontend
  - Razor Pages for routing, server side request management
  - React for UI, to allow reuse of code and build process with the main frontend client app
- React SPA for the main frontend client app
  - bootstrapped by Create React App
  - hosted by the ASP.NET Core app
  - codebase also contains the React frontend parts for the Razor Pages UI
  - Code splitting is used to reduce bundle sizes based on which entry point (SPA or Razor) is used

## Frontend

### Razor Pages

These are fairly minimal server side pages in the ASP.NET Core App, used exclusively for **Identity** interactions that preferably shouldn't be in an external client, such as IdentityProvider **Login** and **Logout** routes.

They are Razor Pages, but they actually each bootstrap a small React app for the page, to allow for component reuse with the main frontend.

The Razor Pages (and their PageModels) are in `Pages/` as per ASP.NET Core convention.

The JavaScript source for these mini apps is kept with the rest of the React Client App source, and building and serving it is managed the same way.

The main entrypoint is in `ClientApp/src/apps/razor`.

### React Client App

This is the actual frontend. It's a typical `create-react-app` based React application.

The main entrypoint is in `ClientApp/src/apps/spa`.

### Running and Building

Because the code for both frontend "apps" lives together, the process of working with both is the same:

In local development, ASP.NET Core's SPA tools will take care of building and HMR via a Webpack Dev server.

In any other environment, the frontend bundle needs building before the ASP.NET Core app is run or published.

This is typically done in CI, but the process is as follows:

#### On the command line:

In the `ClientApp/` directory, **below** `Directory.csproj`:

1. `npm i`
1. `npm run build`

Once the built files exist at `ClientApp/build/`, they will be included by `dotnet publish` and the resulting published App will serve both the SPA and the JS files used by Razor Pages correctly.
