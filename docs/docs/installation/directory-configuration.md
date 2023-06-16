---
title: Configuration
sidebar_position: 2
slug: /configuration
---

Below is a Sample App Settings configuration for the Directory, includes all options* and defaults.

*Third party settings aren't necessarily included or exhaustive, such as default ASP.NET Core settings for logging, Kestrel etc., or other libraries such as Serilog.

`// TODO: Details on how to configure ASP.NET Core?`

```yaml
# -----------------------
# Directory Configuration
# -----------------------
ConnectionStrings:
  Default: "" # ODBC style connection string to a PostgreSQL database

  # Optional if you want Hangfire tables in a different DB
  Hangfire: "" # ODBC style connection string to a PostgreSQL database

  # Only needed for the Submissions REST API functionality
  AzureStorage: "" # Azure Storage Account Connection String

# It's possible to pull navbar items from a Wordpress instance
# Because the original Directory did it.
# This functionality may be deprecated in future
WordpressMenuUrl: ""

SiteProperties:
  # Text replacement
  ServiceName: Biobanking Directory
  PageTitle: Biobanking Directory
  LegalEntity: Legal Entity
  EmailSignature: Biobanking Directory

  # Email addresses / links
  ContactUsUrl: https://example.com
  ContactAddress: contact@example.com
  SupportAddress: support@example.com

  # Homepage / GUI Features
  ClientSessionTimeout: 1200000 # 20 mins in milliseconds
  AlternateHomepage: false
  HotjarEnabled: false

  # Google Analytics Tracking
  GoogleAnalyticsEnabled: false
  GoogleAnalyticsTrackingCode: ''
  GoogleTagId: ''

  # Google Recaptcha v2 Invisible API credentials
  GoogleRecaptchaSecret: ''
  GoogleRecaptchaPublicKey: ''

  # Can an admin of a suspended biobank still access it themselves?
  AllowSuspendedBiobanks: true

AnnualStatistics:
  StartYear: '2015'

Publications:
  # Used by the GUI to search Publications
  # and by the Publications Worker to link Publications to Biobanks
  # (if the worker is active)
  EpmcApiUrl: https://www.ebi.ac.uk/europepmc/

Hangfire:
  SchemaName: Hangfire

ElasticSearch:
  ApiBaseUrl: http://localhost:9200

  # If sharing ES Servers for different environments
  # set these differently per environment :)
  DefaultCollectionsSearchIndex: collections
  DefaultCapabilitiesSearchIndex: capabilities

  # Optional if auth isn't required on the ES instance
  # (e.g. in dev, or if heavily firewalled)
  Username: ''
  Password: ''

OutboundEmail:
  FromName: No Reply
  FromAddress: noreply@example.com
  ReplyToAddress: "" # if left empty, `FromAddress` will be used
  
  Provider: local # see below for options

  # If Provider == "local"
  LocalPath: ~/temp

  # If Provider == "sendgrid"
  SendGridApiKey: ""

  # If Provider == "smtp"
  SmtpHost: "" # SMTP host name
  SmtpPort:    # SMTP port
  SmtpUsername: "" # SMTP username
  SmtpPassword: "" # SMTP password
  SmtpSecureSocketEnum: # for example, assign 2 to implement SslOnConnect
  # Secure socket options
  # 1 - Auto
  # 2 - SslOnConnect
  # 3 - StartTls
  # 4 - StartTlsWhenAvailable

  # More information can be found here
  # http://www.mimekit.net/docs/html/T_MailKit_Security_SecureSocketOptions.htm

# ----------------------
# REST API Configuration
# ----------------------

# Only needed if allowing external use of the REST API (i.e. issuing tokens)
JWT:
  # Used to sign JWTs issued by the backend
  # and verify those in requests
  Secret: "" # A suitable value can be generated using the crypto cli

# --------------------
# Worker Configuration
# --------------------

# Which Workers are enabled
Workers:
  HangfireRecurringJobs: []
  # `analytics` - regularly consume Google Analytics data for dashboarding
  # `aggregator` - aggregate REST API Submissions into GUI Collections
  # `publications` - regularly check for EPMC publications associated with biobanks in the Directory
  # `submissions-expiry` - expire stale Submissions to the REST API

# Only needed if using the Analytics Worker
# These settings configure the use of the Google Analytics API
# to fetch analytics stats so they can be dashboarded within the Directory GUI
Analytics:
  # If specified, all Analytics Data will be filtered by the provided hostname.
  # Usually the hostname of the Directory application.
  FilterHostname: ''

  # Number of Organisations to include in the ranking for Organisation reports
  MetricThreshold: 10

  # Number of Event groups that originated from the same location on a particular day
  # above which will be excluded from plots
  EventThreshold: 30
  StartDate: '2016-01-01'
  GoogleAnalyticsViewId: '' # View ID to fetch analytics data from

  # A JSON Service Account Key for Google Analytics Reporting API v4
  # https://developers.google.com/analytics/devguides/reporting/core/v4/authorization
  GoogleAnalyticsReportingKey: ''

# Only needed if using the Aggregator Worker
Aggregator:
  # ID for the OntologyTerm used in collections for Non-Extracted Samples
  NonExtractedOntologyTerm: ''

  # Mappings between (ContentMethod, ContentId) of a Sample => MacroscopicAssessment
  MacroscopicAssessmentMappings: []
```
