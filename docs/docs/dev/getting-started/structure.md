---
sidebar_position: 2
---

# Projects Structure

The `src/` folder contains roughly project or app categorised folders, which also have Visual Studio Solutions in. Each Solution contains all the relevant projects including dev dependencies, so it's generally a good idea to use the solution of the project or app area you are doing work on.

| Folder                        | Description
| ----------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------
| `Core/`                       | .NET 6 Central Class Libary of shared code with minimal shared dependencies                                                                       |
| `Core/Jobs`                   | .NET 6 Collection of Worker Jobs used for background processes                                                                                    |
| `Data/`                       | EF Core 6 Database Context, Entity classes representing tables in the database, and Migrations for the directory database                                                                             |
| `Submissions/`                | [.NET 6 Web App](/dev/directory/overview), the core functionality of the Tissue Directory, and workers for bulk submission of data to the directory database                                                                  |
| `Omop/`                | .NET 6 OMOP Database Context                                                                  |
