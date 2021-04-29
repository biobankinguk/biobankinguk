# Publications

## About
The `Publication Service` is an aggregator that queries the [Europe PubMed Central API](https://europepmc.org/RestfulWebService) to find potential articles associated with organizations that are registered on the Directory.

The publication service is utilised by the Directory to gain quick access to potential articles associated to a given Biobank. This functionality can be found in the Publication tab in the Biobank Admin panel (ADAC).

Due to the volume of articles hosted by EPMC, this aggregator hosts its own subset of article metadata, to then be queried by the Directory in real-time.

<p align="center">
  <img src="./readme/project-diagram.png" width="48%" />
</p>

Ultimately, the aggregator sites between the Directory and EPMC API - retrieving and caching relevant data.

## Project Structure
The project is broken into two parts. A class library `Publications`, and an Azure Function App service implementation `PublicationsAzureFunctions`. This offers the possiblity to migrate the service to a different platform if required.

The class library contains all the function for the Publication service. This includes service layers to connect to the EPMC API, Directory API and to its own database instance.

The Azure function app is a deployment project that wraps the service layer into two functions. One `Batch` function that excutes daily and one `Endpoint` function that executes on HTTP Trigger.

## Configuration

The Function App some application settings (e.g. as an Environment Variable, or in the Azure Portal):

```yml
EpmcApiUrl: "https://www.ebi.ac.uk/europepmc/"
```

## Testing

Testing requires your Directory having some active, registered Biobanks.

Initally, we should manually trigger the `Batch` function, to populate the database with some articles.

The function can be selected from the Function App from `Functions > Functions` and then manually triggered from `Developer > Code + Test > Test/Run`. Once executed, you should see a sucess message:

```
Executed 'BatchFunction' (Succeeded, Id=, Duration=22986ms)
```

Next we can trigger the `EPMC` function via a HTTP GET request. From the `EPMCFunction` we can grab the endpoint url using `Get Function Url`. Supplying a valid Biobank name, we will get a HTTP 200 response, if the service has an available articles.

```json
[
    {
        "id":123123123,
        "title":"Demo Title",
        "authorString":"James Fleming",
        "journalTitle":"Nature",
        "pubYear":2020,
        "doi":null
    }
]
```
