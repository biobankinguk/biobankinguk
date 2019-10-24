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
2. Build the frontend
3. Set Environment Configuration Settings

Details for each step follow below.

## 1. Initialise the database

The App uses Entity Framework Core with Code First Migrations.

Currently (at this stage of development) migrations are manual, but they will become automatic in future.

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

## 2. Build the frontend

The Directory Web Project contains two distinct frontend areas, which both have `npm` build steps:

### Razor Pages

These are fairly minimal server side pages in the ASP.NET Core App, used exclusively for **Identity** interactions that shouldn't be in an external client, such as IdentityProvider **Login** and **Logout** routes.

They are Razor Pages, but they actually each bootstrap a small React app for the page, to allow for component reuse with the main frontend. As such, the javascript bundle containing the React apps needs building, which done by `npm`.

There are two build tasks - `dev` and `build`.
- `dev` should mostly be used for local development.
- `build` does an optimised production build, which is used in CI when producing actual releases.

#### In Visual Studio:

1. **Configure External Tools** so your `PATH` version of `npm` is prioritised over VS's own.
1. Install the **NPM Task Runner** extension.

VS should now run the `dev` task whenever you build the ASP.NET Core project.

> ℹ You can run the `dev` or `build` tasks manually from **Task Runner Explorer** if you need to.

#### On the command line:

In the same directory as `Directory.csproj`

1. `npm i`
1. `npm run dev`

### React Client App

This is the actual frontend. It's a typical `create-react-app` based React application.

In local development, ASP.NET Core's SPA tools will take care of building and HMR via a Webpack Dev server.

In any other environment, the React App needs building before the .NET Core app is run or published.

This is typically done in CI, but the process is as follows:

#### On the command line:

In the `ClientApp/` directory, **below** `Directory.csproj`:

1. `npm i`
1. `npm run build`

Once the built files exist at `ClientApp/build/`, they will be included by `dotnet publish` and the resulting published App will serve the SPA correctly.

## 3. Set Environment Configuration Settings

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

| Setting Key            | Description                                                                  | Example |
| ---------------------- | ---------------------------------------------------------------------------- | ------- |
| `SuperAdminSeedPassword` | A password used to seed the SuperAdmin user on first run. **Must not** be empty. | `test`  |
| `TrustedClients:upload-api:secret` | The Client Secret for the Upload API Client. | `test` |
