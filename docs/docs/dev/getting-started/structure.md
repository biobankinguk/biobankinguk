---
sidebar_position: 2
---

# Projects Structure

## `app/`

The `app/` folder contains application entrypoint projects for the stack. Currently there is only one application: the Directory.

| Folder | Description |
| - | - |
| `Directory/` | [.NET 6 Web App](/dev/directory), the core functionality of the Tissue Directory, and [workers for bulk submission of data](/dev/submissions/worker-jobs) to the directory database |

## `lib/`

The `lib/` folder contains class library projects which support the functionality of the stack and the potential for multiple applications sharing functionality.

| Folder | Description |
| - | - |
| `Core/` | .NET 6 Central Class Libary of shared code with minimal shared dependencies |
| `Core/Jobs` | .NET 6 Collection of Worker Jobs used for background processes |
| `Data/` | EF Core 6 Database Context, Entity classes representing tables in the database, and Migrations for the directory database |
| `Omop/` | .NET 6 OMOP Database Context |
