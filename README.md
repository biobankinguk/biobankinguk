# 🏥 BiobankingUK Tissue Directory

This monorepo contains all the applications and tools for the complete BiobankingUK Tissue Directory stack.

The stack is used to run the **UKCRC Tissue Directory and Co-ordination Centre** Directory and its peripheral services, but is open source and can be used in part or wholly for other instances.

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
