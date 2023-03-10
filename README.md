# 🏥 BiobankingUK Tissue Directory

This monorepo contains all the applications and tools for the complete BiobankingUK Tissue Directory stack.

The stack is used to run the **UKCRC Tissue Directory and Co-ordination Centre** Directory and its peripheral services, but is open source and can be used in part or wholly for other instances.

## 👩‍💻 Start Developing

If you're doing some development on the repository, you'll want to look at the repository structure guide below this, and follow instructions for the particular area of the codebase you're working with.

However, you will almost certainly need a development instance of the directory database to work with.

Here's how to get that going.

### Prerequisites

- .NET SDK 6.x or newer
- PostgreSQL Server

### Steps

1. Install local repo tooling for .NET
   - `dotnet tool restore` anywhere inside the repo
1. Run database migrations
   - Change directory next to the `src` folder.
   - You need to specify the DbContext, and target the `Data` project, using the `Submissions` project as the startup project.
   - `dotnet ef database update -c ApplicationDbContext -p Data/Data.csproj -s Submissions/Api/Api.csproj`
1. Seed Reference Data
   - optionally create modified seed data
       - Copy seed data files from `/sample-seed-data` to another location and modify them
   - Change directory next to the `Submissions/Api/Api.csproj`
   - `dotnet run -- ref-data seed -d <path to seed data directory>`
       - e.g. to use the sample data: `dotnet run -- ref-data seed -d ../../../sample-seed-data`
       - use `--help` for other options

That's all the database preparation.

> ℹ Don't forget to take a look at the `README` for the specific application area you're working in.

Happy developing!

## 📂 Guide to Repository Structure

The `src/` folder contains roughly project or app categorised folders, which also have Visual Studio Solutions in. Each Solution contains all the relevant projects including dev dependencies, so it's generally a good idea to use the solution of the project or app area you are doing work on.

### Repository structure

- `.github/` - GitHub Actions and Workflows for building and releasing the stack, managing the repo
- `docs/` - Source for the documentation site
- `elastic-search/` - Configuration scripts to setup the required elastic search instance
- `sample-seed-data/` - Sample Reference data for the directory database
- `scripts/` - Various scripts for Directory management
- `sql/` - Various SQL scripts used to add/repair database records
- `src/` - Projects Source Code, detailed below

### Projects (in `src/`)

| Folder                        | Description                                                                                                                                       | `README` |
| ----------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- | -------- |
| `Core/`                       | .NET 6 Central Class Libary of shared code with minimal shared dependencies                                                                       | ❌       |
| `Core/Jobs`                   | .NET 6 Collection of Worker Jobs used for background processes                                                                                    | ❌       |
| `Data/`                       | EF Core 6 Database Context, Entity classes representing tables in the database, and Migrations for the directory database                                                                             | ❌       |
| `Submissions/`                | .NET 6 Web App, the core functionality of the Tissue Directory, and workers for bulk submission of data to the directory database                                                                  | ✔        |
| `Omop/`                | .NET 6 OMOP Database Context                                                                  | ❌        |

## 🧾 License

Source code in this repository is licensed under the MIT license, unless otherwise specified. Content licenses and attributions are retained adjacent to and in reference to the relevant content where required by the license.

## 👩‍🏭 Contribute

Currently contributions are not accepted from outside of the University of Nottingham Digital Research Service team.

This may change in future.
