# Directory Web Application

The Directory project is the core piece of the BiobankingUK stack. Everything else in the stack serves to optionally augment the Directory.

It consists of an .NET Core MVC web application, backed by a PostgreSQL database, which interacts with an Elasticsearch server.

## Setup

> ℹ Complete the instructions in [Getting Started](overview) first.

1. Enable [Corepack](https://nodejs.org/api/corepack.html)
   - Simply run `corepack enable` in your cli

1. Install Node Packages
   - Change directory to the `Frontend` folder
   - Run `pnpm install`

1. Add a new user
   - Change directory next to the `Api.csproj` file.
   - Run `dotnet run -- users add <EMAIL> <FULL-NAME> -r <ROLES> -p <PASSWORD>`
   - For example: `dotnet run -- users add admin@example.com Admin -r SuperUser DirectoryAdmin -p Password1!`
   - For local dev use you probably want the roles: `SuperUser`, and `DirectoryAdmin`
     - being a `SuperUser` doesn't automatically put you in the `DirectoryAdmin` role too. Sorry.
   - See [users CLI documentation](/dev/cli/users#add)
1. Check Email Configuration
   - by default for local development, the app will write emails to `~/temp`
   - See [Email Sending](email-sending)

## Optional steps

To use the Search functionality:

1. Setup a local Elasticsearch instance
   - See [Elasticsearch](elasticsearch), Docker recommended
