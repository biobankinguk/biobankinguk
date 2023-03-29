---
sidebar_position: 1
---

# Overview

This is a Command Line tool useful for managing some application access tasks.

## Usage

It can be run from the source, or a build (distributed or built locally).

Running from source requires the .NET `6.0.x` SDK or newer.

### Base Command

Use the appropriate command from here and then add the command you want to run as documented below.

- `dotnet run --` to run from source next to the `Directory.csproj`

Get help with the `--help` option.

## Commands

The command line interface to administer the application, run commands as `dotnet run -- <COMMAND>`

- [`api-clients`](cli/api) : Actions for managing BiobankingUK ApiClients
- [`crypto`](cli/crypto) : Actions for working with secure identifiers
- [`ref-data`](cli/ref-data) : Actions for managing BiobankingUK Reference Data
- [`users`](cli/users) : Actions for managing BiobankingUK Users

### Options

- `-c` | `--connection-string` `<CONNECTION-STRING>` : Database Connection String if not specified in Configuration

## Configuration

It's a .NET Application, it supports the [usual Configuration Sources](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/) e.g.

- User Secrets
- `appsettings.json`
- Environment Variables

### Configuration Values

| Key | Description | Example |
|-|-|-|
| `ConnectionStrings:Default` | The Database connection string for commands which need it. | `Server=(localdb)\mssqllocaldb;Database=Biobanks` |
| `Serilog` | Serilog configuration | [Serilog docs](https://github.com/serilog/serilog-settings-configuration) |
