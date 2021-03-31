# UKCRC Tissue Directory Reporting Service

The code in this repository can generate reports on the Google Analytics data associated with the UKCRC TDCC Directory.

Reports can either be run for the entirety of the directory, or for each registered biobank / sample resource individually.

# Overview

## Google Analytics key file
For authentication with Google Analytics, a key file (JSON) is necessary in order to work with OAuth2. More details on that here: https://developers.google.com/analytics/devguides/reporting/core/v4/authorization


# Code & Module Structure
The main structure for the code is as follows:
```
AnalyticsFunction.cs # Generates biobank report from HTTP request
- DirectoryAnalyticsFunction.cs # Generates TDCC directory report from HTTP request
- BatchFunction.cs # Downloads analytics data for biobanks and TDCC directory from Google Analytics
```

to use the service (generate a report), 
simply send a GET request using the function route e.g. for a biobank report :
```
http://{hostname}/api/GetAnalyticsReport/{biobankId}/{year}/{quarter}/{period}?code={apikey}";
```
and for a directory report :
```
http://{hostname}/api/GetDirectoryAnalyticsReport/{year}/{quarter}/{period}?code={apikey}";
```
where 
```
biobankId: string # biobank/organistion external Id (e.g. GBR-1-1)
year: int, # year the reporting period should end
quarter: int, # quarter in which the reporting period should end (usually reports end with the most recent completed quarter)
period: int, # number of quarters the reports should cover (usually 8)
apikey :  string # can be found for example from the azure function's portal,
``` 


## Configuration
Definition of App configs:
```
"AnalyticsApikey": JSON, # json object in Google Analytics key file, see section above 
"AnalyticsViewid": int, # ID of the Google Analytics view
"FilterbyHost": boolean # if set to true, analytics data would be filtered to include only those from the specified hostname
"DirectoryHostname": string # specifies hostname when FilterbyHost is enabled e.g. ("directory.biobankinguk.org")
"StartDate": string, # start date of the timeframe to download Google Analytics data for (format: yyyy-mm-dd)
"MetricThreshold": int, # number of biobanks to include in the ranking for biobank reports (default: 10)
"EventThreshold": int, #  number of event groups that originated from the same location on a particular day above which will be excluded from plots default: 30)
"ConnectionStrings:Default": string # connection string for the (azure) database used to store the analytics data
```


# Notes
- Code adapted from 'biobanks.analytics' Azure DevOps repo (written in python). See for more information (relevant UoN people should have access)