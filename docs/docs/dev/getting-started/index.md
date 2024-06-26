---
sidebar_position: 1
---

# Getting Started

## Prerequisites

- .NET 6.0 SDK
- Node.js `>=16.10` (with `corepack` enabled)
  - just run `corepack enable` once to enable it globally for your node installation
- PostgreSQL 14 in some form

Optionally:

- Docker

## Database Setup

If you're doing some development on the repository, you'll want to look at the [repository structure guide](getting-started/structure), and follow instructions for the particular area of the codebase you're working with.

However, you will almost certainly need a development instance of the directory database to work with.

The application stack interacts with a PostgreSQL database, and uses code-first migrations for managing the database schema.

When setting up a new environment, or running a newer version of the codebase if there have been schema changes, you need to run migrations against your database server.

Here's how to get that going.

1. Install local Entity Framework tooling for .NET
   - `dotnet tool restore` anywhere inside the repo
1. Run database migrations
   - Change directory to the root of the repository.
   - You need to specify the DbContext, and target the `Data` project, using the `Directory` project as the startup project.
   - `dotnet ef database update -c ApplicationDbContext -p lib/Data/Data.csproj -s app/Directory/Directory.csproj`
1. Seed Reference Data
   - optionally create modified seed data
       - Copy seed data files from `/sample-seed-data` to another location and modify them
   - Change directory next to the `app/Directory/Directory.csproj`
   - `dotnet run -- ref-data seed -d <path to seed data directory>`
       - e.g. to use the sample data: `dotnet run -- ref-data seed -d ./sample-seed-data`
       - use `--help` for other options
   - See [seed documentation](cli/ref-data#seed) for further guidance.

## Working with JavaScript

This monorepo uses [pnpm](https://pnpm.io) workspaces to manage JS dependencies and scripts.

Basically, where you might normally use `npm` or `yarn`, please use `pnpm` commands instead.

You don't need to install anything special; Corepack will.

A brief pnpm cheatsheet is provided [here](getting-started/pnpm-cheatsheet).
