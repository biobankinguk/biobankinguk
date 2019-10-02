This contains the new Directory App.

Currently referred to as "Core Services", it provides core services used by the current Directory, and other projects such as the Upload API.

Services provided by the App at this time:

- Authentication
- Reference Data CRUD Endpoints

# Getting Started

Mainly, just open the Solution in Visual Studio, or the folder in your favourite editor, such as VS Code.

## Frontend

The Directory Web Project does contain a few "server-side" pages, for authentication only. These are Razor Pages, and they depend on a small frontend bundle, for which npm manages dependencies and tasks.

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
