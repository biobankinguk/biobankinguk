# Publications

Publications is a class library that contains all the function for the Publication service. This is primarily to connect to the EuropePMC API and collate publication data for each Organisation in the Directory. This is stored in a SQL database so it can be queried by an API endpoint.

## PublicationsMockApp

A small Web App that hosts a local SQL database and exposes a `./mock?organisationName={}` API endpoint.

## Setup
`appsettings.json`
```json
"ConnectionStrings": {
    "Publications": "Database Url",
    "Directory": "Directory API Url",
    "EuropePMC": "Europe PMC API Url"
}
```

`Startup.cs`
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Configure Database Context
    services.AddDbContext<PublicationDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("Publications"))
    );

    // DI of required services
    services.AddScoped<IPublicationService, PublicationService>();
    services.AddScoped<IBiobankService, BiobankWebService>();
    services.AddScoped<IEPMCService, EMPCWebService>();

    // Register hosted (background) service
    services.AddHostedService<FetchPublicationsService>();
}
           
```

