# biobankinguk

This repository aims to contain all code pertaining to the **UKCRC Tissue Directory and Co-ordination Centre**; everything that lives under the domain `biobankinguk.org` should be here if it is under source control.

In practice, there is ongoing work to migrate the live Tissue Directory and API codebase here.

This repository will contain a few applications, some shared code, and some other useful bits and pieces for development.

Distinct applications may have their own `README` files for guidance on getting started. There is a guide below.

# Getting Started

If you're doing some development on the repository, you'll want to look at the repository structure guide below, and follow instructions for the particular area of the codebase you're working with.

However, you will almost certainly need a development instance of the central directory database to work with.

Here's how to get that going.

## Prerequisites

- .NET SDK 5.0.0 or newer
- SQL Server (LocalDB is fine)

## Steps
1. Install local repo tooling for .NET
    - `dotnet tool restore` anywhere inside the repo
1. Run database migrations
    - Change directory next to the `Data.csproj` (`/src/Data/Data.csproj`)
    - `dotnet dotnet-ef database update -- <connection string>`
    - For development, steal the connection string from one of the App's development configs.
1. Seed Reference Data
    - Copy seed data files from `/sample-seed-data` to `/src/DataSeed/data`
    - optionally modify the seed data
    - Change directory next to the `DataSeed.csproj` (`/src/DataSeed/DataSeed.csproj`)
    - `dotnet run`
    - Currently the tool does not report completion and waits after `PreservationType` is seeded.
    - `TODO: update instructions for DataSeed releases as well as local version`

That's all the database preparation. Happy developing!


# Guide to Repository Structure

The `src/` folder contains roughly project or app categorised folders, which also have Visual Studio Solutions in. Each Solution contains all the relevant projects including dev dependencies, so it's generally a good idea to use the solution of the project or app area you are doing work on.

## Directory structure:

- `.azure/pipelines/` - Azure Pipelines configurations for conditionally building the different applications and libraries in the repo
- `elastic-search/` - Configurational scripts to setup the required elastic search instance
- `sample-seed-data/` - Reference data for the Directory
- `scripts/` - Various scripts for Directory management
- `sql/` - Various SQL scripts used to add/repair database records
- `src/` - Projects Source Code

| Folder | Description | `README` |
| - | - | - |
| `Analytics/` | .Net Core 3.1 Azure Function generating Google Analytics reports for the Directory | ✔ 
| `Directory/` | ASP.NET 4.8 Web App - The core functionality of biobankinguk.org | ✔ 
| `Publications/` | .Net Core 3.1 Azure Function which fetches and stores relevant metadata on articles published by Directory users on EuropePMC | ✔ 

# License

Source code in this repository is licensed under the MIT license, unless otherwise specified. Content licenses and attributions are retained adjacent to and in reference to the relevant content where required by the license.

# Contribute

Currently contributions are not accepted from outside of the University of Nottingham Digital Research Service team.

This may change in future.
