# UKCRC Tissue Directory Reporting pipeline

The code in this repository can generate reports on the Google Analytics data associated with the UKCRC TDCC Directory.

Reports can either be run for the entirety of the directory, or for each registered biobank / sample resource individually.

# Pipeline overview

## Google Analytics key file
For authentication with Google Analytics, a key file (JSON) is necessary in order to work with OAuth2. More details on that here: https://developers.google.com/analytics/devguides/reporting/core/v4/authorization


# Code & Module Structure
The main structure for the code is as follows:
```
AnalyticsFunction.cs # Generates biobank report from HTTP request
- DirectoryAnalyticsFunction.cs # Generates TDCC directory report from HTTP request
- BatchFunction.cs # Downloads analytics data for biobanks and TDCC directory from Google Analytics
```

to run the pipeline (generate a report), 
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
"analytics-apikey": JSON, # json object in Google Analytics key file, see section above 
"analytics-viewid": int, # ID of the Google Analytics view
"DirectoryUrl": string # URL of TDCC site to retrieve biobank information via api call e.g ("http://directory.biobankinguk.org/")
"filterby-host": boolean # if set to true, analytics data would be filtered to include only those from the specified hostname
"directory-hostname": string # specifies hostname when filterby-host is enabled e.g. ("directory.biobankinguk.org")
"start-date": string, # start date of the timeframe to download Google Analytics data for (format: yyyy-mm-dd)
"metric-threshold": int, # number of biobanks to include in the ranking for biobank reports (default: 10)
"event-threshold": int, #  number of event groups that originated from the same location on a particular day above which will be excluded from plots default: 30)
"sqldb-username": string # username for the (azure) database used to store the analytics data
"sqldb-password": string # password for the (azure) database used to store the analytics data
"analyticsdb_connection": string # connection string for the (azure) database used to store the analytics data with the username and password excluded (to be parsed in) in the format e.g "Server=servername;Initial Catalog=dbname;Persist Security Info=False;User ID={0};Password={1};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```


# Further misc comments on the pipeline
- Code adapted from 'biobanks.analytics' Azure DevOps repo (written in python). See for more information (relevant UoN people should have access)

- when running EF migrations, the username and password should be filled/passed into "analyticsdb_connection" (as you'd have for a normal connection string) and passed as an env variable. This is expected by AnalyticsDbContext.cs during creation.