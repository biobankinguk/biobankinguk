# Email Sending

The Directory sends emails particularly around Account Management (for example password resets).

It supports sending via **SendGrid**, or local mail services.

It works out what to do as follows:

- If `Provider` is `sendgrid` in `appsettings.OutboundEmail`, it will try to use **SendGrid** if an API key is available
- If `Provider` is `local` in `appsettings.OutboundEmail`, it will use local

The above is achieved by conditionally resolving `IEmailService` with either a basic `EmailService` or a `SendGridEmailService`.

## Basic `EmailService`

If the basic `EmailService` is used, it will behave differently based on the configuration in `appsettings.OutboundEmail`.

- By default, it will write mails to disk, at `~/temp`.
- It can be configured to use any SMTP Server
  - **SendGrid** is typically preferable though

The default on-disk solution is the easiest for local development, as there is no delay and no receiving mailbox required.
