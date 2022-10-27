using Biobanks.Aggregator;
using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Analytics;
using Biobanks.Analytics.Services;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.Data;
using Biobanks.IdentityModel.Helpers;
using Biobanks.Omop.Context;
using Biobanks.Publications.Services.Contracts;
using Biobanks.Shared.Services.Contracts;
using Core.AzureStorage;
using Core.Submissions.Models.OptionsModels;
using Core.Submissions.Services;
using Core.Submissions.Services.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Biobanks.Aggregator.Services;
using Biobanks.Shared.Services;
using Biobanks.Publications.Services;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Search.Legacy;
using Biobanks.Submissions.Api.Services.Submissions.Contracts;
using Biobanks.Submissions.Api.Services.Submissions;
using Hangfire;
using ClacksMiddleware.Extensions;
using Swashbuckle.AspNetCore.SwaggerUI;
using UoN.AspNetCore.VersionMiddleware;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.JsonConverters;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System;
using Biobanks.Submissions.Api.Auth.Basic;
using Biobanks.Submissions.Api.Auth.Entities;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Default");

//databases
builder.Services.AddDbContext<OmopDbContext>(options =>
options.UseNpgsql("Omop"));

builder.Services.AddDbContext<BiobanksDbContext>(options =>
    options.UseSqlServer(connectionString,
    sqlServerOptions => sqlServerOptions.CommandTimeout(300000000)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<BiobanksDbContext>();

builder.Services.AddRazorPages();

builder.Configuration.AddJsonFile("Settings/LegacyMaterialTypes.json", optional: true);
builder.Configuration.AddJsonFile("Settings/LegacyStorageTemperatures.json", optional: true);

// local config
var jwtConfig = builder.Configuration.GetSection("JWT").Get<JwtBearerConfig>();
var workersConfig = builder.Configuration.GetSection("Workers").Get<WorkersOptions>() ?? new();
var hangfireConfig = builder.Configuration.GetSection("Hangfire").Get<HangfireOptions>() ?? new();

builder.Services.AddOptions()
    .Configure<IISServerOptions>(opts => opts.AllowSynchronousIO = true)
                .Configure<JwtBearerConfig>(builder.Configuration.GetSection("JWT"))
                .Configure<AggregatorOptions>(builder.Configuration.GetSection("Aggregator"))
                .Configure<AnalyticsOptions>(builder.Configuration.GetSection("Analytics"))
                .Configure<WorkersOptions>(builder.Configuration.GetSection("Workers"))
                .Configure<HangfireOptions>(builder.Configuration.GetSection("Hangfire"))
                .Configure<MaterialTypesLegacyModel>(builder.Configuration.GetSection("MaterialTypesLegacyModel"))
                .Configure<StorageTemperatureLegacyModel>(builder.Configuration.GetSection("StorageTemperatureLegacyModel"));

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.Audience = JwtBearerConstants.TokenAudience;
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = JwtBearerConstants.TokenIssuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = Crypto.GenerateSigningKey(jwtConfig.Secret),
                        RequireExpirationTime = true
                    };
                })
                .AddBasic(opts => opts.Realm = "biobankinguk-api");

builder.Services.AddControllersWithViews(opts => opts.SuppressOutputFormatterBuffering = true)
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    o.JsonSerializerOptions.Converters.Add(new JsonNumberAsStringConverter());
                });

builder.Services.AddAuthorization(o =>
{
    o.DefaultPolicy = AuthPolicies.IsAuthenticated;
    o.AddPolicy(nameof(AuthPolicies.IsTokenAuthenticated),
        AuthPolicies.IsTokenAuthenticated);
    o.AddPolicy(nameof(AuthPolicies.IsBasicAuthenticated),
        AuthPolicies.IsBasicAuthenticated);
    o.AddPolicy(nameof(AuthPolicies.IsSuperAdmin),
        AuthPolicies.IsSuperAdmin);
});

builder.Services.AddSwaggerGen(opts =>
{
    // Docs details
    opts.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "BiobankingUK Directory API",
            Version = "v1"
        });

    // Doc generation sources
    opts.EnableAnnotations();

    // Auth configuration
    opts.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic"
    });
    opts.AddSecurityDefinition("jwtbearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    opts.OperationFilter<SecurityRequirementsOperationFilter>();

    // Allow grouping across controllers
    opts.TagActionsBy(api =>
    {
        var tag = api.GroupName
            ?? (api.ActionDescriptor as ControllerActionDescriptor)?.ControllerName;

        if (tag is null) throw new InvalidOperationException("Unable to determine tag for endpoint.");
        return new[] { tag };
    });
    opts.DocInclusionPredicate((name, api) => true);

    var xmlFilename = $"swagger.xml"; //File name should match the documation being called in Api.csproj
    opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
    .AddHttpClient()
    .AddMemoryCache()
    .AddTransient<IBlobWriteService, AzureBlobWriteService>( // TODO: Merge Blob Read and Write services
    _ => new(builder.Configuration.GetConnectionString("AzureStorage")))
    .AddTransient<IBlobReadService, AzureBlobReadService>(
    _ => new(builder.Configuration.GetConnectionString("AzureStorage")))
                .AddTransient<IQueueWriteService, AzureQueueWriteService>(
                    _ => new(builder.Configuration.GetConnectionString("AzureStorage")))

                // Local Services
                .AddTransient<ISubmissionService, SubmissionService>()
                .AddTransient<IDiagnosisWriteService, DiagnosisWriteService>()
                .AddTransient<IDiagnosisValidationService, DiagnosisValidationService>()
                .AddTransient<ITreatmentWriteService, TreatmentWriteService>()
                .AddTransient<ITreatmentValidationService, TreatmentValidationService>()
                .AddTransient<ISampleWriteService, SampleWriteService>()
                .AddTransient<ISampleValidationService, SampleValidationService>()
                .AddTransient<IReferenceDataReadService, ReferenceDataReadService>() // TODO: Merge ReferenceDataReadService and ReferenceDataService
                .AddTransient<IErrorService, ErrorService>()


                .AddTransient<ICollectionAggregatorService, CollectionAggregatorService>()
                .AddTransient<ISampleService, SampleService>()
                .AddTransient<IOrganisationService, OrganisationService>()
                .AddTransient<IAggregationService, AggregationService>()
                .AddTransient(typeof(Biobanks.Shared.Services.Contracts.IReferenceDataService<>), typeof(Biobanks.Shared.Services.ReferenceDataService<>))

                .AddTransient<IReferenceDataAggregatorService, ReferenceDataAggregatorService>()
                .AddTransient<ISampleService, SampleService>()
                .AddTransient<IAggregationService, AggregationService>()

                .AddTransient<IPublicationJobService, PublicationJobService>()
                .AddTransient<IAnnotationService, AnnotationService>()
                .AddTransient<IEpmcService, EpmcWebService>()

                .AddTransient<IDirectoryReportGenerator, DirectoryReportGenerator>()
                .AddTransient<IOrganisationReportGenerator, OrganisationReportGenerator>()
                .AddTransient<IReportDataTransformationService, ReportDataTransformationService>()
                .AddTransient<IAnalyticsService, AnalyticsService>()
                .AddTransient<IGoogleAnalyticsReportingService, GoogleAnalyticsReportingService>()

                .AddTransient<ISubmissionExpiryService, SubmissionExpiryService>()
                .AddTransient<IRegistrationDomainService, RegistrationDomainService>()
                .AddTransient<IDiseaseStatusService, DiseaseStatusService>()

                // Reference Data
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AccessCondition>, AccessConditionService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AgeRange>, AgeRangeService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AnnualStatistic>, AnnualStatisticService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AnnualStatisticGroup>, AnnualStatisticGroupService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AssociatedDataProcurementTimeframe>, AssociatedDataProcurementTimeframeService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AssociatedDataTypeGroup>, AssociatedDataTypeGroupService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<CollectionPercentage>, CollectionPercentageService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<CollectionStatus>, CollectionStatusService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<CollectionType>, CollectionTypeService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<ConsentRestriction>, ConsentRestrictionService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<County>, CountyService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<Country>, CountryService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<DonorCount>, DonorCountService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<MacroscopicAssessment>, MacroscopicAssessmentService>()
                .AddTransient<IMaterialTypeService, MaterialTypeService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<MaterialTypeGroup>, MaterialTypeGroupService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<PreservationType>, PreservationTypeService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<RegistrationReason>, RegistrationReasonService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<SampleCollectionMode>, SampleCollectionModeService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<ServiceOffering>, ServiceOfferingService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<Sex>, SexService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<SopStatus>, SopStatusService>()
                .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<Biobanks.Entities.Shared.ReferenceData.StorageTemperature>, StorageTemperatureService>();

//Directory Services
if (bool.Parse(builder.Configuration["DirectoryEnabled:Enabled"]) == true)
{
    builder.Services
        .AddTransient<IPublicationService, PublicationService>()
        .AddTransient<IOrganisationDirectoryService, OrganisationDirectoryService>() //TODO: merge or resolve OrganisationDirectory and Organisation Services
        .AddTransient<IContentPageService, ContentPageService>()
        .AddTransient(typeof(Biobanks.Shared.Services.Contracts.IReferenceDataService<>))
        .AddTransient<IConfigService, ConfigService>()
        .AddTransient<ICollectionService, CollectionService>()
        .AddTransient<IOntologyTermService, OntologyTermService>()
        .AddTransient<ITokenLoggingService, TokenLoggingService>()
        .AddTransient(typeof(IGenericEFRepository<>), typeof(IGenericEFRepository<>))
        .AddTransient<IBiobankReadService, BiobankReadService>()
        .AddTransient<IBiobankIndexService, BiobankIndexService>()
        .AddTransient<ILogoStorageProvider, SqlServerLogoStorageProvider>()
        .AddTransient<IBiobankWriteService, BiobankWriteService>()
        .AddTransient<ILogoStorageProvider, SqlServerLogoStorageProvider>()
        .AddTransient<IIndexProvider, LegacyIndexProvider>()
        .AddTransient<INetworkService, NetworkService>()
        .AddTransient<IAnalyticsReportGenerator, AnalyticsReportGenerator>()
        .AddTransient<IBiobankWriteService, BiobankWriteService>()
    //   .AddTransient<ElasticCapabilityIndexProvider, ICapabilityIndexProvider>();

    // Reference Data
        .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AssociatedDataType>, AssociatedDataTypeService>()
        .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<Funder>, FunderService>();

}

// Conditional services
if (workersConfig.HangfireRecurringJobs.Any() || workersConfig.QueueService == WorkersQueueService.Hangfire)
{
    var hangfireConnectionString = builder.Configuration.GetConnectionString("Hangfire");

    builder.Services.AddHangfire(x => x.UseSqlServerStorage(
        !string.IsNullOrWhiteSpace(hangfireConnectionString)
            ? connectionString
            : builder.Configuration.GetConnectionString("Default"),
        new() { SchemaName = hangfireConfig.SchemaName }));
}

switch (workersConfig.QueueService)
{
    case WorkersQueueService.AzureQueueStorage:
        builder.Services.AddTransient<IBackgroundJobEnqueueingService, AzureQueueService>();
        break;
    // case WorkersQueueService.Hangfire: // this is the default!
    default:
        builder.Services
            .AddTransient<IBackgroundJobEnqueueingService, HangfireQueueService>()
            .AddTransient<IRejectService, RejectService>()
            .AddTransient<ICommitService, CommitService>();
        break;
}


var app = builder.Build();

app.GnuTerryPratchett()
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseRouting();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app
    // Simple public middleware
    .UseStatusCodePages()
    .UseVersion()

    // Swagger
    .UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            swaggerDoc.Servers = new List<OpenApiServer> {
                            new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } });
    })
    .UseSwaggerUI(c =>
    {
        c.RoutePrefix = string.Empty; // serve swagger ui from root ;)
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        c.SupportedSubmitMethods(SubmitMethod.Get);
    })

                // Endpoint Routing
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers().RequireAuthorization();

                    if (app.Environment.IsDevelopment())
                    {
                        endpoints.MapHangfireDashboard("/hangfire");
                    }
                    else
                    {
                        var dashboardOptions = new DashboardOptions()
                        {
                            Authorization = Array.Empty<IDashboardAuthorizationFilter>() // Removes Default Local-Auth Filter
                        };

                        endpoints
                            .MapHangfireDashboard("/hangfire", dashboardOptions)
                            .RequireAuthorization(nameof(AuthPolicies.IsSuperAdmin));
                    }
                })

                // Hangfire Server
                .UseHangfireDashboard();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
