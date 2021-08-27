# Postman Testing Suite

This folder contains all the automated Postman tests used to verify the Submissions API is working as intended. The tests are split up into different collections depending on their domain. This is to make testing specific components of the API Sutie easier, and makes updating and adding new tests simpler.

|##| Collection                 | Description                                                   |
|--| -------------------------- | ------------------------------------------------------------- |
|01| User Authentication        | Authenticates the user, acquiring a `user-token`              |
|02| Smoke Test                 | Checks if all required service are live                       |
|03| Identity Service           | Test authentication endpoint and some authenticated endpoints |
|04| Data Sample Validation     | Check validation of all Sample fields                         |
|05| Data Diagnosis Validation  | Check validation of all Diagnosis fields                      |
|06| Data Treatment Validation  | Check validation of all Treatment fields                      |
|07| Data Sample Errors         | Check correct error reporting on invalid Samples              |
|08| Data Diagnosis Errors      | Check correct error reporting on invalid Diagnoses            |
|09| Data Treatment Errors      | Check correct error reporting on invalid Treatment            |
|10| Status Service             | Check responses on status endpoint for uploaded submissions   |
|11| Reject                     | Test rejection of submissions                                 |
|12| Commit                     | Test commital of submissions                                  |
|13| Submission Limits          | Submission limits and stress tests (10k, 500k Submissions)    |
|14| Post Test Cleaning         | Clears all submitted, unproccessed data                       |

## Running With Newman CLI (Recommended)

Newman is the CLI tool created by the Postman team, to be able to run Postman collections via the command line. It is the recommended way to quickly and robustly run tests against any Submissions API environment.

More information about Newman can be found on the [Postman website](https://learning.postman.com/docs/running-collections/using-newman-cli/command-line-integration-with-newman/). 

### **Prerequisites**

Newman is a `Node.js` tool and therefore requires `npm` to be installed. 

To install and setup the Newman CLI globally, run

```bash
$ npm install -g newman
```

To test Newman has been installed correctly

```bash
$ newman run -h
```

This will display all available options and flags of the Newman CLI. For more details on the CLI options, check out the [Newman documentation](https://learning.postman.com/docs/running-collections/using-newman-cli/command-line-integration-with-newman/#options).

### **Getting Started**

To begin, the Postman environment must be properly configured to the user's API credentials by editing `environment.postman_environment.json`. This contains all environmental variables used by the Postman collections during testing.

| Key                   | Value                     | Description                   |
|-----------------------|---------------------------|-------------------------------|
| api-url               | https://localhost:5001    | URL of the base API endpoint  |
| directory-url         | https://localhost:44300   | URL of the Directory          |
| user-client-id        |                           | Generated API User ID         |
| user-client-secret    |                           | Generated API User Secret     |
| user-biobank-id       |                           | Organisation Internal ID      |

By default, the environment is configured to run against a local test environment. The user credentials will have to be supplied; these can be generated under the `Bulk Submissions` section on an Organisations Admin page.

Once configured, the user needs to be authenticated by the API such that a `user-token` can be generated. The `user-token` is required by all subsequent collections runs for all authenticated requests.

The `user-token` is generated by running the `Biobanks API - 01 User Authentication` collection.

```bash
$ newman run "Biobanks API - 01 User Authentication.postman_collection.json" -e "environment.postman_environment.json" --export-collection "environment.postman_environment.json"
```

In this case, we want to export the environment such that the `user-token` is written to the environment configuraton.

### **Running A Collection**

To run a collection, the Newman CLI needs reference to the collection and the environment json files

```bash
$ newman run "<collection.json>" -e "<environment.json>"
```

For testing on a local environment, where a security certificate may not be configured, the `--insecure` flag must be used to avoid additional errors.

## Running In Postman

These tests can also be imported, and ran via Postman. In Postman use the importer under `File > Import > Folder`. This will import all tests and the template environment to be used in Postman.