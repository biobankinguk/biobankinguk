# 🌐 Directory Web App

The Directory project is the core piece of the BiobankingUK stack. Everything else in the stack serves to optionally augment the Directory.

It consists of an ASP.NET MVC5 web application, backed by a SQL Server database, which interacts with an Elastic Search server.

# 👩‍💻 Start Developing

Prerequisites:

- .NET 4.8
- SQL Server in some form
- ❔ Optionally the Docker Desktop client.

Assumptions:

- You'll be using the latest Visual Studio to interact with the codebase at large.

## ‼ Essential steps

> ℹ Complete the instructions in the repository `README` first!

1. Run Identity database migrations
   - Instructions below
1. Add yourself as a user
   - Run the `add-user.sql` script
   - For local dev use you probably want the roles: `SuperUser`, and `ADAC`
     - being a `SuperUser` doesn't automatically put you in the `ADAC` role too. Sorry.
   - When you run the app, use the `Forgot password` functionality to set your password.
1. Check Email Configuration
   - by default for local development, the app will write emails to `C:\Temp`
   - make sure the path exists, or change the configuration
   - Instructions below

## ❔ Optional steps

To use Search functionality:

1. Setup a local Elastic Search instance
   - Instructions below, Docker recommended

# 🙍‍♂️ Identity Database Migrations

- The Directory uses ASP.NET Identity 2.x to manage user authentication and authorization against a local userstore.
- The structure of that local userstore is created by Entity Framework 6 code first migrations.
- It **does not** use Automatic Migrations; all schema changes must have a migration added using `add-migration`.
- it **does not** run any migrations automatically (i.e. `update-database` doesn't happen on App Startup)

## Apply migrations to a local database

If there are schema changes you don't have locally, you need to apply them.

In **Package Manager Console**:
1. Set "Default Project" to `Identity`
1. run `update-database`

## Applying migrations to a non-local environment

Use `ef6.exe` to perform the migrations.

## ⚠ Making Schema changes

It should be rare, but if you make a schema change to entities in the `Identity` Project, you'll need to add and run a migration.

> ℹ Migrations for the `Entities` Project are handled by the top-level `Data` Project, not the Directory!

### Adding new migrations

You need to record the change in a migration, so it can be applied to databases.

In **Package Manager Console**:
1. Set "Default Project" to `Identity`
1. run `add-migration <name>`

# 🔍 Elastic Search

You'll need an Elastic Search `7.x` instance.

## 😊 With Docker

- `docker-compose up` inside the `elastic-search/` directory will provide a suitable dev search server.
- payloads for index configuration and example queries are also in the `elastic-search/` directory.

## 😐 Without Docker

ElasticSearch can be installed locally. It depends on Java.

Kibana et al are unnecessary for local development - Postman or similar can be used to hit the ES REST API.

Interacting with the Elastic Search REST API is documented in `elastic-search/README.md`

# ✉ Email Sending

The Directory sends emails particularly around Account Management (password resets etc).

It supports sending via **SendGrid**, or the `System.Net` mail services.

It works out what to do as follows:

- If `UseSendGrid` is `true` in `Web.Config`, it will try to use **SendGrid** if an API key is available
  - If `UseKeyVault` is `true` in `Web.Config` it will get the API key from **Azure KeyVault**, else it will take it from `Web.Config`.
- If `UseSendGrid` is `false` or no API key can be found, it will use `System.Net`

The above is achieved by conditionally resolving `IEmailService` with either a basic `EmailService` or a `SendGridEmailService`.

## Basic `EmailService`

If the basic `EmailService` is used, it will behave differently based on the `System.Net` mail settings in `Web.Config`.

- By default, it will write mails to disk, at `C:\Temp\`, so you should make sure that path exists.
- It can be configured to use any SMTP Server
  - **SendGrid** is typically preferable though

The default on-disk solution is the easiest for local development, as there is no delay and no receiving mailbox required.
