This contains the new Directory App.

Currently referred to as "Core Services", it provides core services used by the current Directory, and other projects such as the Upload API.

Services provided by the App at this time:

- Authentication
- Reference Data CRUD Endpoints

# Getting Started

Mainly, just open the Solution in Visual Studio, or the folder in your favourite editor, such as VS Code.

Extra guidance follows:

## Database

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

## Frontend

The Directory Web Project contains two distinct frontend areas:

- **Razor Pages**
    - These are server side pages in the ASP.NET Core App
    - They are fairly minimal and are used exclusively for **Identity** interactions that shouldn't be in an external client, such as IdentityProvider **Login** and **Logout** routes.
- **React Client App**
    - This constitutes the actual frontend Directory application.

### Razor Pages

As summarised above, there are a few "server-side" pages, for authentication only. These are Razor Pages, and they depend on a small frontend bundle, for which npm manages dependencies and tasks.

You'll want to build the frontend bundles for these pages to work correctly:

- Have `npm` 5+
- In Visual Studio:
    - **Configure External Tools** so your `PATH` version of `npm` is prioritised over VS's own.
    - Install the **NPM Task Runner** extension.
    - VS should now run the `build` task whenever you build the ASP.NET Core project.
    - You can run the `build` task manually from **Task Runner Explorer** if you need to.
- On the command line:
    - In the same directory as `Directory.csproj`
    - `npm i`
    - `npm run build`

### React Client App

As summarised above, this is the real frontend. It's a typical `create-react-app` based React application.

In local development, ASP.NET Core's SPA tools will take care of building and HMR via Webpack's Dev server.

In any other environment, the React App needs building before the .NET Core app is run or published. This is typically done in CI, but the process is as follows:

- Have `npm` 5+
- On the command line:
    - In the `ClientApp/` directory, **below** `Directory.csproj`
    - `npm i`
    - `npm run build`

Once the built files exist at `ClientApp/build/`, `dotnet publish` will include them and the resulting published App will serve the SPA correctly.