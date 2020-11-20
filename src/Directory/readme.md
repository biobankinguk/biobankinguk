# Biobanks Directory

In overview, the Biobanks Directory project consists of an ASP.NET MVC5 web application, backed by a SQL Server database, which interacts with an Elastic Search server.

## Getting Started

There are some essential, and optional, things to get started with the Biobanks Directory application.

This readme covers the details, and also provides this summary to help you get started with which details you need to look for.

Prerequisites:

- .NET 4.8
- SQL Server in some form
- Optionally the Docker Desktop client.

Assumptions:

- You'll be using the latest Visual Studio to interact with the codebase at large.

Essential steps to get started locally:

1. Run database migrations (see notes below)
   - This generates the database structure, and seeds a little bit of essential data.
1. Seed the database Reference Data
   1. Copy the data you want to seed to a `data/` folder relative to the `DataSeed` application exe
      - CSV's named the same as the tables.
      - See `/sample-seed-data` for samples)
   1. Run the DataSeed application
      - If you just run it from Visual Studio, everything should be fine for a local dev environment
      - For further details, it has its own `README` in `./DataSeed/README.md`
1. Add yourself as a user
    - Run the `add-user.sql` script
    - For local dev use you probably want the roles: `SuperUser`, and `ADAC`
      - being a `SuperUser` doesn't automatically put you in the `ADAC` role too. Sorry.
    - When you run the app, use the `Forgot password` functionality to set your password.

Optional steps to get started locally:

To use Search functionality:

1. Setup a local Elastic Search instance (see notes below, Docker recommended)

## Local development

You'll need an Elastic Search 7.x instance running locally.

Notes follow:

- `docker-compose up` inside the `elastic-search/` directory will provide a suitable dev search server.
- payloads for index configuration and example queries are also in the `elastic-search/` directory.

Alternatively, if Docker is not installed, ElasticSearch can be installed locally (which means installing Java and going through the same setup followed on the other environments)

Kibana et al are unnecessary for local development - Postman or similar can be used to hit the ES REST API.

Interacting with the Elastic Search REST API is documented in `elastic-search/README.md`

## Notes on Database Migrations

- The Directory uses Entity Framework 6's Code-First Migrations.
- It **does not** use Automatic Migrations; all schema changes must have a migration added using `add-migration`.
- It uses the default initialiser, which will create a db (running all migrations) if it does not exist; otherwise does nothing.
  - i.e. it **does not** run new migrations on existing databases automatically.
- However, because **Hangfire** talks to the db before that initial create migration is run, you actually always have to run migrations manually, regardless of whether the db exists.

### Handling Schema changes locally

If you make (or need to apply) a schema change, you need to know what entities have changed and which context they belong to: `BiobanksDbContext` or `UserStoreDbContext`.

Then you should do the following in Visual Studio's **Package Manager Console**:

1. Ensure Package Manager Console is targeting the correct project, for the correct Entities/Context:
   - `BiobanksDbContext` - `Data`
   - `UserStoreDbContext` - `Identity`

#### Adding new migrations

If you have made a schema change, you need to record the change in a migration, so it can be applied to databases.

In **Package Manager Console** run `add-migration <name>` ensuring the Package Manager Console "Default Project" is correct, as noted above.

#### Apply migrations to a local database

If there are schema changes you don't have locally, you need to apply them.

In **Package Manager Console** run `update-database` ensuring the Package Manager Console "Default Project" is correct, as noted above.

### Applying migrations to a non-local environment

This can be done a variety of ways in EF6.

The ways we recommend are either using the `ef6.exe` to perform the migration.

## Notes on emails

The Directory sends emails particularly around Account Management (password resets etc).

It supports sending via **SendGrid**, or the `System.Net` mail services.

It works out what to do as follows:

- If `UseSendGrid` is `true` in `Web.Config`, it will try to use **SendGrid** if an API key is available
  - If `UseKeyVault` is `true` in `Web.Config` it will get the API key from **Azure KeyVault**, else it will take it from `Web.Config`.
- If `UseSendGrid` is `false` or no API key can be found, it will use `System.Net`

The above is achieved by conditionally resolving `IEmailService` with either a basic `EmailService` or a `SendGridEmailService`.

### Basic `EmailService`

If the basic `EmailService` is used, it will behave differently based on the `System.Net` mail settings in `Web.Config`.

- By default, it will write mails to disk, at `C:\Temp\`, so you should make sure that path exists.
- In most environments that actually use `System.Net`, it uses the University of Nottingham SMTP Server
  - Those environments have been whitelisted for use of the SMTP Server: Any other environments wishing to use the University SMTP Server will require whitelisting.
  - **SendGrid** is probably preferable to further whitelisting

The default on-disk solution is the easiest for local development, as there is no delay and no receiving mailbox required.


