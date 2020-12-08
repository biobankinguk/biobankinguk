
## Submission Expiry Job

An Azure Function that is scheduled, on a timer, to clear out submission's data and mark it as expired. This function is ran daily and will expire submissions older than a configured number of days

### Configuration

The Expiry Function is configured using via a `settings.json` file or via an overriding `local.settings.json` file when developing on a local machine,

The settings can also be overrided in the App Settings in the Azure Portal for the Azure Function resource.

```json
{
    "Values": {
        // Instrumentation Key For Using App Insights (Disabled if empty)
        "APPINSIGHTS_INSTRUMENTATIONKEY": "",
        
        // Maximum Age (In Days) Until A Submission Expires (Default: 30 days)
        "expiryDays": 30 
    },
    "ConnectionStrings": {
        // API Database Connection String
        "DefaultConnection": ""
    }
}

```
