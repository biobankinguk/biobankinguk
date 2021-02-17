# Biobanks.DataSeed

This app is designed for data seeding to new environments.

# Getting started

- Download a binary from the [latest DevOps build](https://dev.azure.com/UniversityOfNottingham/DRS/_build/latest?definitionId=283&branchName=master) artifacts
- Run it directly from source in Visual Studio
- Publish it locally and run the output:
  - e.g. `dotnet publish DataSeed.csproj -c Release -o publish`

## Prerequisites

It's a .NET 4.7.2 app (because it uses the Directory's supporting Data library, and EF6, and it's just easier if it matches the Directory...).

As a result, it's Windows only.

- Have .NET Framework 4.7.2
- Ensure the machine you're running on can connect to a target database environment
  - e.g. if it's in the Cloud, ensure IP whitelisting or firewall rules or whatever

## Usage

### Seed Data

`> Biobanks.Directory.DataSeed seed`

Will try and find identically named CSV files for the Directory's "Reference Data" tables in an adjacent `data/` folder, and INSERT the data into the DB.

Sample files can be found in `sample-seed-data/` in the root of this repository

## Configuration

It's built like a .NET Core app, using the .NET Generic Host.

This means:

- it reads App Configuration from `appsettings.json` adjacent to the entrypoint `.exe` / `.dll`
- it reads config from Environment Variables prefixed `DOTNET_`
  - e.g. `DOTNET_RefDataConnectionString` can set the database connection string
- it reads Host Configuration (e.g. `environment`) from Command Line arguments:
  - `--RefDataConnectionString=<value>`
- If you run it from Visual Studio, it can use the values in `Properties/launchSettings.json`

### Configuration Values

| Key | Description |
|-|-|
| `RefDataConnectionString` | The Connection String to use to connect to the database. |
