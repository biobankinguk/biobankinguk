using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Biobanks.Aggregator;
using Biobanks.Aggregator.Services;
using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Analytics;
using Biobanks.Analytics.Services;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Omop.Context;
using Biobanks.Publications.Services;
using Biobanks.Publications.Services.Contracts;
using Biobanks.Search.Contracts;
using Biobanks.Search.Elastic;
using Biobanks.Search.Legacy;
using Biobanks.Shared.Services;
using Biobanks.Shared.Services.Contracts;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Auth.Basic;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Extensions;
using Biobanks.Submissions.Api.Filters;
using Biobanks.Submissions.Api.JsonConverters;
using Biobanks.Submissions.Api.Middleware;
using Biobanks.Submissions.Api.Services;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.Submissions;
using Biobanks.Submissions.Api.Services.Submissions.Contracts;
using Biobanks.Submissions.Api.Utilities.IdentityModel;
using ClacksMiddleware.Extensions;
using cloudscribe.Web.SiteMap;
using Core.AzureStorage;
using Core.Submissions.Models.OptionsModels;
using Core.Submissions.Services;
using Core.Submissions.Services.Contracts;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using UoN.AspNetCore.VersionMiddleware;

namespace Biobanks.Submissions.Api.Startup.Web;

public static class WebEntrypoint
{
  public static async Task Run(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("Default");

//databases
    builder.Services.AddDbContext<OmopDbContext>(options =>
      options.UseNpgsql("Omop"));

    builder.Services.AddDbContext<ApplicationDbContext>(o =>
    {
      // migration bundles don't like null connection strings (yet)
      // https://github.com/dotnet/efcore/issues/26869
      // so if no connection string is set we register without one for now.
      // if running migrations, `--connection` should be set on the command line
      // in real environments, connection string should be set via config
      // all other cases will error when db access is attempted.
      var connectionString = builder.Configuration.GetConnectionString("Default");
      if (string.IsNullOrWhiteSpace(connectionString))
        o.UseNpgsql();
      else
        o.UseNpgsql(connectionString,
          o => o.EnableRetryOnFailure());
    });

//identity
    builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
      .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.AddRazorPages();

    builder.Configuration.AddJsonFile("Settings/LegacyMaterialTypes.json", optional: true);
    builder.Configuration.AddJsonFile("Settings/LegacyStorageTemperatures.json", optional: true);

// local config
    var jwtConfig = builder.Configuration.GetSection("JWT").Get<JwtBearerConfig>();
    var workersConfig = builder.Configuration.GetSection("Workers").Get<WorkersOptions>() ?? new();
    var hangfireConfig = builder.Configuration.GetSection("Hangfire").Get<HangfireOptions>() ?? new();
    var elasticConfig = builder.Configuration.GetSection("ElasticSearch").Get<ElasticsearchConfig>() ?? new();
    var sitemapConfig = builder.Configuration.GetSection("NavigationOptions");


    builder.Services.AddOptions()
      .Configure<IISServerOptions>(opts => opts.AllowSynchronousIO = true)
      .Configure<SitePropertiesOptions>(builder.Configuration.GetSection("SiteProperties"))
      .Configure<JwtBearerConfig>(builder.Configuration.GetSection("JWT"))
      .Configure<AggregatorOptions>(builder.Configuration.GetSection("Aggregator"))
      .Configure<AnalyticsOptions>(builder.Configuration.GetSection("Analytics"))
      .Configure<WorkersOptions>(builder.Configuration.GetSection("Workers"))
      .Configure<HangfireOptions>(builder.Configuration.GetSection("Hangfire"))
      .Configure<MaterialTypesLegacyModel>(builder.Configuration.GetSection("MaterialTypesLegacyModel"))
      .Configure<StorageTemperatureLegacyModel>(builder.Configuration.GetSection("StorageTemperatureLegacyModel"))
      .Configure<ElasticsearchConfig>(builder.Configuration.GetSection("Elasticsearch"));

//sitemap + breadcrumbs
    builder.Services.AddScoped<ISiteMapNodeService, NavigationTreeSiteMapNodeService>();
    builder.Services.AddCloudscribeNavigation(sitemapConfig);
    builder.Services
      .AddScoped<cloudscribe.Web.Navigation.INavigationTreeBuilder,
        cloudscribe.Web.Navigation.JsonNavigationTreeBuilder>();

    builder.Services.Configure<MvcOptions>(options =>
    {
      options.CacheProfiles.Add("SiteMapCacheProfile",
        new CacheProfile
        {
          Duration = 100
        });
    });

    builder.Services.AddApplicationInsightsTelemetry();

    builder.Services.AddEmailSender(builder.Configuration);

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

    builder.Services.AddControllersWithViews(opts =>
      {
        opts.SuppressOutputFormatterBuffering = true;
        opts.Filters.Add<RedirectAntiforgeryValidationFailedResult>();
      })
      .AddViewLocalization()
      .AddJsonOptions(o =>
      {
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        o.JsonSerializerOptions.Converters.Add(new JsonNumberAsStringConverter());
      });

    builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(10); });

    builder.Services.AddAuthorization(o =>
    {
      o.DefaultPolicy = AuthPolicies.IsAuthenticated;
      o.AddPolicy(nameof(AuthPolicies.IsTokenAuthenticated),
        AuthPolicies.IsTokenAuthenticated);
      o.AddPolicy(nameof(AuthPolicies.IsBasicAuthenticated),
        AuthPolicies.IsBasicAuthenticated);
      o.AddPolicy(nameof(AuthPolicies.CanAccessHangfireDashboard),
        AuthPolicies.CanAccessHangfireDashboard);
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
      .AddTransient<IAbstractCrudService, AbstractCrudService>()
      .AddTransient<IAggregationService, AggregationService>()
      .AddTransient<IAnalyticsService, AnalyticsService>()
      .AddTransient<IAnalyticsReportGenerator, AnalyticsReportGenerator>()
      .AddTransient<IAnnotationService, AnnotationService>()
      .AddTransient<IBiobankService, BiobankService>()
      .AddTransient<IBiobankIndexService, BiobankIndexService>()
      .AddTransient<IBiobankWriteService, BiobankWriteService>()
      .AddTransient<ICapabilityService, CapabilityService>()
      .AddTransient<ICollectionAggregatorService, CollectionAggregatorService>()
      .AddTransient<ICollectionService, CollectionService>()
      .AddTransient<IConfigService, ConfigService>()
      .AddTransient<IContentPageService, ContentPageService>()
      .AddTransient<IEpmcService, EpmcWebService>()
      .AddTransient<IErrorService, ErrorService>()
      .AddTransient<IDiagnosisWriteService, DiagnosisWriteService>()
      .AddTransient<IDiagnosisValidationService, DiagnosisValidationService>()
      .AddTransient<IDirectoryReportGenerator, DirectoryReportGenerator>()
      .AddTransient(typeof(IGenericEFRepository<>), typeof(GenericEFRepository<>))
      .AddTransient<IGoogleAnalyticsReportingService, GoogleAnalyticsReportingService>()
      .AddTransient<ILogoStorageProvider, SqlServerLogoStorageProvider>()
      .AddTransient<INetworkService, NetworkService>()
      .AddTransient<IOntologyTermService, OntologyTermService>()
      .AddTransient<IOrganisationReportGenerator, OrganisationReportGenerator>()
      .AddTransient<IOrganisationService, OrganisationService>()
      .AddTransient<IOrganisationDirectoryService,
        OrganisationDirectoryService>() //TODO: merge or resolve OrganisationDirectory and Organisation Services
      .AddTransient<IPublicationJobService, PublicationJobService>()
      .AddTransient<IPublicationService, PublicationService>()
      .AddTransient<IRecaptchaService, RecaptchaService>()
      .AddTransient<IReportDataTransformationService, ReportDataTransformationService>()
      .AddTransient(typeof(Biobanks.Shared.Services.Contracts.IReferenceDataService<>),
        typeof(Biobanks.Shared.Services.ReferenceDataService<>))
      .AddTransient<IReferenceDataAggregatorService, ReferenceDataAggregatorService>()
      .AddTransient<IReferenceDataReadService,
        ReferenceDataReadService>() // TODO: Merge ReferenceDataReadService and ReferenceDataService
      .AddTransient<IRegistrationDomainService, RegistrationDomainService>()
      .AddTransient<ISampleService, SampleService>()
      .AddTransient<ISampleSetService, SampleSetService>()
      .AddTransient<ISampleWriteService, SampleWriteService>()
      .AddTransient<ISampleValidationService, SampleValidationService>()
      .AddTransient<ISubmissionExpiryService, SubmissionExpiryService>()
      .AddTransient<ISubmissionService, SubmissionService>()
      .AddTransient<ITokenLoggingService, TokenLoggingService>()
      .AddTransient<ITreatmentWriteService, TreatmentWriteService>()
      .AddTransient<ITreatmentValidationService, TreatmentValidationService>()

      // Search Services
      .AddTransient<ICollectionSearchProvider>(
        sp => new ElasticCollectionSearchProvider(
          elasticConfig.ElasticsearchUrl,
          (elasticConfig.DefaultCollectionsSearchIndex, elasticConfig.DefaultCapabilitiesSearchIndex),
          elasticConfig.Username,
          elasticConfig.Password
        )
      )
      .AddTransient<ICollectionIndexProvider>(
        sp => new ElasticCollectionIndexProvider(
          elasticConfig.ElasticsearchUrl,
          (elasticConfig.DefaultCollectionsSearchIndex, elasticConfig.DefaultCapabilitiesSearchIndex),
          elasticConfig.Username,
          elasticConfig.Password
        )
      )
      .AddTransient<ICapabilitySearchProvider>(
        sp => new ElasticCapabilitySearchProvider(
          elasticConfig.ElasticsearchUrl,
          (elasticConfig.DefaultCollectionsSearchIndex, elasticConfig.DefaultCapabilitiesSearchIndex),
          elasticConfig.Username,
          elasticConfig.Password
        )
      )
      .AddTransient<ICapabilityIndexProvider>(
        sp => new ElasticCapabilityIndexProvider(
          elasticConfig.ElasticsearchUrl,
          (elasticConfig.DefaultCollectionsSearchIndex, elasticConfig.DefaultCapabilitiesSearchIndex),
          elasticConfig.Username,
          elasticConfig.Password
        )
      )
      .AddTransient<ISearchProvider, LegacySearchProvider>()
      .AddTransient<IIndexProvider, LegacyIndexProvider>()

      // Reference Data
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AccessCondition>,
        AccessConditionService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AgeRange>,
        AgeRangeService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AnnualStatistic>,
        AnnualStatisticService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AnnualStatisticGroup>,
        AnnualStatisticGroupService>()
      .AddTransient<
        Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AssociatedDataProcurementTimeframe>,
        AssociatedDataProcurementTimeframeService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AssociatedDataTypeGroup>
        ,
        AssociatedDataTypeGroupService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<AssociatedDataType>,
        AssociatedDataTypeService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<CollectionPercentage>,
        CollectionPercentageService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<CollectionStatus>,
        CollectionStatusService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<CollectionType>,
        CollectionTypeService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<ConsentRestriction>,
        ConsentRestrictionService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<County>,
        CountyService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<Country>,
        CountryService>()
      .AddTransient<IDiseaseStatusService, DiseaseStatusService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<DonorCount>,
        DonorCountService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<Funder>,
        FunderService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<MacroscopicAssessment>,
        MacroscopicAssessmentService>()
      .AddTransient<IMaterialTypeService, MaterialTypeService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<MaterialTypeGroup>,
        MaterialTypeGroupService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<PreservationType>,
        PreservationTypeService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<RegistrationReason>,
        RegistrationReasonService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<SampleCollectionMode>,
        SampleCollectionModeService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<ServiceOffering>,
        ServiceOfferingService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<Sex>, SexService>()
      .AddTransient<Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<SopStatus>,
        SopStatusService>()
      .AddTransient<
        Biobanks.Submissions.Api.Services.Directory.Contracts.IReferenceDataService<
          Biobanks.Entities.Shared.ReferenceData.StorageTemperature>, StorageTemperatureService>();

//Directory Services
    if (bool.Parse(builder.Configuration["DirectoryEnabled:Enabled"]) == true)
    {
      builder.Services
        .AddTransient<IBiobankReadService, BiobankReadService>();
    }

// Conditional services
    if (workersConfig.HangfireRecurringJobs.Any() || workersConfig.QueueService == WorkersQueueService.Hangfire)
    {
      var hangfireConnectionString = builder.Configuration.GetConnectionString("Hangfire");


      builder.Services.AddHangfire(x => x.UsePostgreSqlStorage(
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

// Set cache isolated from running of the app
    using (var scope = app.Services.CreateScope())
    {
      var configCache = scope.ServiceProvider
        .GetRequiredService<IConfigService>();

      await configCache.PopulateSiteConfigCache();
    }

    app.GnuTerryPratchett();

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
    app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
    app.UseStaticFiles();


//Authenticated users have their last login value updated to now
    app.UseDirectoryLogin();

    app
      // Simple public middleware
      .UseVersion()

      // Swagger
      .UseSwagger(c =>
      {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
          swaggerDoc.Servers = new List<OpenApiServer>
          {
            new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
          });
      })
      .UseSwaggerUI(c =>
      {
        c.RoutePrefix = "swagger";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        c.SupportedSubmitMethods(SubmitMethod.Get);
      });

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSession();

    app.MapControllers().RequireAuthorization();

    if (app.Environment.IsDevelopment())
    {
      app.MapHangfireDashboard("/hangfire");
    }
    else
    {
      var dashboardOptions = new DashboardOptions()
      {
        Authorization = Array.Empty<IDashboardAuthorizationFilter>() // Removes Default Local-Auth Filter
      };

      app
        .MapHangfireDashboard("/hangfire", dashboardOptions)
        .RequireAuthorization(nameof(AuthPolicies.CanAccessHangfireDashboard));
    }

// Hangfire Server
    app.UseHangfireDashboard();

    app.MapRazorPages();

    app.MapControllerRoute(
      name: "AreasDefault",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");
    
    await app.RunAsync();
  }
}