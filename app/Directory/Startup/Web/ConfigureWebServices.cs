using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Biobanks.Aggregator;
using Biobanks.Aggregator.Services;
using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Analytics;
using Biobanks.Analytics.Services;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.AzureStorage;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Auth.Basic;
using Biobanks.Directory.Config;
using Biobanks.Directory.Filters;
using Biobanks.Directory.JsonConverters;
using Biobanks.Directory.Search.Contracts;
using Biobanks.Directory.Search.Elastic;
using Biobanks.Directory.Search.Legacy;
using Biobanks.Directory.Services.Directory;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.Submissions;
using Biobanks.Directory.Services.Submissions.Contracts;
using Biobanks.Directory.Startup.ConfigureServicesExtensions;
using Biobanks.Directory.Utilities.IdentityModel;
using Biobanks.Omop.Context;
using Biobanks.Publications.Services;
using Biobanks.Publications.Services.Contracts;
using Biobanks.Shared.Services;
using Biobanks.Shared.Services.Contracts;
using Biobanks.Submissions.Models.OptionsModels;
using Biobanks.Submissions.Services;
using Biobanks.Submissions.Services.Contracts;
using cloudscribe.Web.SiteMap;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AccessConditionService = Biobanks.Directory.Services.Directory.AccessConditionService;
using StorageTemperature = Biobanks.Data.Entities.Shared.ReferenceData.StorageTemperature;

namespace Biobanks.Directory.Startup.Web;

public static class ConfigureWebServices
{
  public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder b)
  {
    // Local Configuration
    var dbConnectionString = b.Configuration.GetConnectionString("Default");
    var siteConfig = b.Configuration.GetSection("SiteProperties").Get<SitePropertiesOptions>() ?? new();
    var jwtConfig = b.Configuration.GetSection("JWT").Get<JwtBearerConfig>();

    // App Options Configuration
    b.Services.AddOptions()
      .Configure<IISServerOptions>(opts => opts.AllowSynchronousIO = true)
      .Configure<SitePropertiesOptions>(b.Configuration.GetSection("SiteProperties"))
      .Configure<PublicationsOptions>(b.Configuration.GetSection("Publications"))
      .Configure<AnnualStatisticsOptions>(b.Configuration.GetSection("AnnualStatistics"))
      .Configure<JwtBearerConfig>(b.Configuration.GetSection("JWT"))
      .Configure<AggregatorOptions>(b.Configuration.GetSection("Aggregator"))
      .Configure<AnalyticsOptions>(b.Configuration.GetSection("Analytics"))
      .Configure<WorkersOptions>(b.Configuration.GetSection("Workers"))
      .Configure<HangfireOptions>(b.Configuration.GetSection("Hangfire"))
      .Configure<MaterialTypesLegacyModel>(b.Configuration.GetSection("MaterialTypesLegacyModel"))
      .Configure<StorageTemperatureLegacyModel>(b.Configuration.GetSection("StorageTemperatureLegacyModel"))
      .Configure<ElasticSearchConfig>(b.Configuration.GetSection("ElasticSearch"));

    // Databases
    b.Services
      .AddDbContext<OmopDbContext>(options =>
        options.UseNpgsql("Omop"))
      .AddDbContext<ApplicationDbContext>(o =>
        o.UseNpgsql(dbConnectionString,
          pgo => pgo.EnableRetryOnFailure()));

    // Identity
    b.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
      .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();
    b.Services.ConfigureApplicationCookie(opts =>
    {
      // TODO: move opts
      opts.ExpireTimeSpan = TimeSpan.FromMilliseconds(siteConfig.ClientSessionTimeout);
      opts.LoginPath = "/Account/Login";
      opts.AccessDeniedPath = "/Account/Forbidden";
      opts.ReturnUrlParameter = "returnUrl";
      opts.SlidingExpiration = true;
    });

    // Sitemap / Breadcrumbs
    b.Services.AddSitemapBreadcrumbs(b.Configuration);

    // MVC
    b.Services.Configure<MvcOptions>(options =>
    {
      // TODO: move opts
      options.CacheProfiles.Add("SiteMapCacheProfile",
        new CacheProfile
        {
          Duration = 100
        });
    });
    b.Services.AddRazorPages();
    b.Services.AddControllersWithViews(opts =>
      {
        opts.SuppressOutputFormatterBuffering = true;
        opts.Filters.Add<RedirectAntiforgeryValidationFailedResult>();
      })
      .AddViewLocalization()
      .AddJsonOptions(o =>
      {
        // TODO: move opts?
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        o.JsonSerializerOptions.Converters.Add(new JsonNumberAsStringConverter());
      });

    // Session
    b.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(10); });

    // Authentication
    b.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddJwtBearer(
        // TODO: move opts
        opts =>
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
      .AddBasic(opts => opts.Realm = "biobankinguk");

    // Authorization
    b.Services.AddAuthorization(o =>
    {
      o.DefaultPolicy = AuthPolicies.IsAuthenticated;
      o.AddPolicy(nameof(AuthPolicies.IsTokenAuthenticated),
        AuthPolicies.IsTokenAuthenticated);
      o.AddPolicy(nameof(AuthPolicies.IsBasicAuthenticated),
        AuthPolicies.IsBasicAuthenticated);
      o.AddPolicy(nameof(AuthPolicies.CanAccessHangfireDashboard),
        AuthPolicies.CanAccessHangfireDashboard);
      o.AddPolicy(nameof(AuthPolicies.IsSuperUser),
        AuthPolicies.IsSuperUser);
      o.AddPolicy(nameof(AuthPolicies.IsDirectoryAdmin),
        AuthPolicies.IsDirectoryAdmin);
      o.AddPolicy(nameof(AuthPolicies.IsBiobankAdmin),
        AuthPolicies.IsBiobankAdmin);
      o.AddPolicy(nameof(AuthPolicies.IsNetworkAdmin),
        AuthPolicies.IsNetworkAdmin);
      o.AddPolicy(nameof(AuthPolicies.HasBiobankRequestClaim),
        AuthPolicies.HasBiobankRequestClaim);
      o.AddPolicy(nameof(AuthPolicies.HasBiobankClaim),
        AuthPolicies.HasBiobankClaim);
      o.AddPolicy(nameof(AuthPolicies.HasNetworkRequestClaim), 
        AuthPolicies.HasNetworkRequestClaim);
      o.AddPolicy(nameof(AuthPolicies.HasNetworkClaim),
        AuthPolicies.HasNetworkClaim);
      o.AddPolicy(nameof(AuthPolicies.CanAdministerSampleSet),
        AuthPolicies.CanAdministerSampleSet);
      o.AddPolicy(nameof(AuthPolicies.CanAdministerCapability),
        AuthPolicies.CanAdministerCapability);
      o.AddPolicy(nameof(AuthPolicies.CanAdministerCollection),
        AuthPolicies.CanAdministerCollection);
    });

    // App Insights
    b.Services.AddApplicationInsightsTelemetry();

    // Email sending
    b.Services.AddEmailSender(b.Configuration);

    // Swagger Docs Generation
    b.Services.AddSwaggerGen(opts =>
    {
      // TODO: move opts!

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

    // General Services
    b.Services
      .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
      .AddHttpClient()
      .AddMemoryCache();

    // Application Services
    b.Services
      .AddAzureStorage(b.Configuration)
      .AddElasticSearch(b.Configuration)
      .AddReferenceDataCrud()
      .AddWorkerJobs(b.Configuration)
      .AddDirectoryGuiServices()
      .AddAggregatorServices()
      .AddDirectoryAnalytics()
      .AddBiobankPublications()
      .AddSubmissionsApiServices()

      // Other Application Service Registrations
      .AddTransient<IOrganisationService, OrganisationService>()
      .AddTransient(typeof(IReferenceDataService<>),
        typeof(ReferenceDataService<>));


    return b;
  }

  private static IServiceCollection AddSitemapBreadcrumbs(
    this IServiceCollection s,
    IConfiguration c)
    => s.AddScoped<ISiteMapNodeService, NavigationTreeSiteMapNodeService>()
      .AddCloudscribeNavigation(c.GetSection("NavigationOptions"))
      .AddScoped<cloudscribe.Web.Navigation.INavigationTreeBuilder,
        cloudscribe.Web.Navigation.JsonNavigationTreeBuilder>();

  private static IServiceCollection AddElasticSearch(this IServiceCollection s, IConfiguration c)
  {
    var elasticConfig = c.GetSection("ElasticSearch").Get<ElasticSearchConfig>() ?? new();

    return s.AddTransient<ICollectionSearchProvider>(
        sp => new ElasticCollectionSearchProvider(
          elasticConfig.ApiBaseUrl,
          (elasticConfig.DefaultCollectionsSearchIndex, elasticConfig.DefaultCapabilitiesSearchIndex),
          elasticConfig.Username,
          elasticConfig.Password
        )
      )
      .AddTransient<ICollectionIndexProvider>(
        sp => new ElasticCollectionIndexProvider(
          elasticConfig.ApiBaseUrl,
          (elasticConfig.DefaultCollectionsSearchIndex, elasticConfig.DefaultCapabilitiesSearchIndex),
          elasticConfig.Username,
          elasticConfig.Password
        )
      )
      .AddTransient<ICapabilitySearchProvider>(
        sp => new ElasticCapabilitySearchProvider(
          elasticConfig.ApiBaseUrl,
          (elasticConfig.DefaultCollectionsSearchIndex, elasticConfig.DefaultCapabilitiesSearchIndex),
          elasticConfig.Username,
          elasticConfig.Password
        )
      )
      .AddTransient<ICapabilityIndexProvider>(
        sp => new ElasticCapabilityIndexProvider(
          elasticConfig.ApiBaseUrl,
          (elasticConfig.DefaultCollectionsSearchIndex, elasticConfig.DefaultCapabilitiesSearchIndex),
          elasticConfig.Username,
          elasticConfig.Password
        )
      )
      .AddTransient<ISearchProvider, LegacySearchProvider>()
      .AddTransient<IIndexProvider, LegacyIndexProvider>();
  }

  private static IServiceCollection AddAzureStorage(this IServiceCollection s, IConfiguration c)
  {
    var azureStorageConnectionString = c.GetConnectionString("AzureStorage");

    // TODO: Merge Blob Read and Write services
    return s.AddTransient<IBlobWriteService, AzureBlobWriteService>(
        _ => new(azureStorageConnectionString))
      .AddTransient<IBlobReadService, AzureBlobReadService>(
        _ => new(azureStorageConnectionString))
      .AddTransient<IQueueWriteService, AzureQueueWriteService>(
        _ => new(azureStorageConnectionString));
  }

  private static IServiceCollection AddReferenceDataCrud(this IServiceCollection s)
  {
    return s.AddTransient<IReferenceDataCrudService<AccessCondition>, AccessConditionService>()
      .AddTransient<IReferenceDataCrudService<AgeRange>, AgeRangeService>()
      .AddTransient<IReferenceDataCrudService<AnnualStatistic>, AnnualStatisticService>()
      .AddTransient<IReferenceDataCrudService<AnnualStatisticGroup>, AnnualStatisticGroupService>()
      .AddTransient<IReferenceDataCrudService<AssociatedDataProcurementTimeframe>,
        AssociatedDataProcurementTimeframeService>()
      .AddTransient<IReferenceDataCrudService<AssociatedDataTypeGroup>, AssociatedDataTypeGroupService>()
      .AddTransient<IReferenceDataCrudService<AssociatedDataType>, AssociatedDataTypeService>()
      .AddTransient<IReferenceDataCrudService<CollectionPercentage>, CollectionPercentageService>()
      .AddTransient<IReferenceDataCrudService<CollectionStatus>, CollectionStatusService>()
      .AddTransient<IReferenceDataCrudService<CollectionType>, CollectionTypeService>()
      .AddTransient<IReferenceDataCrudService<ConsentRestriction>, ConsentRestrictionService>()
      .AddTransient<IReferenceDataCrudService<County>, CountyService>()
      .AddTransient<IReferenceDataCrudService<Country>, CountryService>()
      .AddTransient<IDiseaseStatusService, DiseaseStatusService>()
      .AddTransient<IReferenceDataCrudService<DonorCount>, DonorCountService>()
      .AddTransient<IReferenceDataCrudService<Funder>, FunderService>()
      .AddTransient<IReferenceDataCrudService<MacroscopicAssessment>, MacroscopicAssessmentService>()
      .AddTransient<IMaterialTypeService, MaterialTypeService>()
      .AddTransient<IReferenceDataCrudService<MaterialTypeGroup>, MaterialTypeGroupService>()
      .AddTransient<IReferenceDataCrudService<PreservationType>, PreservationTypeService>()
      .AddTransient<IReferenceDataCrudService<RegistrationReason>, RegistrationReasonService>()
      .AddTransient<IReferenceDataCrudService<SampleCollectionMode>, SampleCollectionModeService>()
      .AddTransient<IReferenceDataCrudService<ServiceOffering>, ServiceOfferingService>()
      .AddTransient<IReferenceDataCrudService<Sex>, SexService>()
      .AddTransient<IReferenceDataCrudService<SopStatus>, SopStatusService>()
      .AddTransient<IReferenceDataCrudService<StorageTemperature>, StorageTemperatureService>();
  }

  private static IServiceCollection AddWorkerJobs(this IServiceCollection s, IConfiguration c)
  {
    var workersConfig = c.GetSection("Workers").Get<WorkersOptions>() ?? new();

    if (workersConfig.HangfireRecurringJobs.Any() || workersConfig.QueueService == WorkersQueueService.Hangfire)
    {
      var hangfireConfig = c.GetSection("Hangfire").Get<HangfireOptions>() ?? new();
      var hangfireConnectionString = c.GetConnectionString("Hangfire");

      s.AddHangfire(x => x.UsePostgreSqlStorage(
        !string.IsNullOrWhiteSpace(hangfireConnectionString)
          ? hangfireConnectionString
          : c.GetConnectionString("Default"),
        new() { SchemaName = hangfireConfig.SchemaName }));
      s.AddHangfireServer();
    }

    switch (workersConfig.QueueService)
    {
      case WorkersQueueService.AzureQueueStorage:
        s.AddTransient<IBackgroundJobEnqueueingService, AzureQueueService>();
        break;
      case WorkersQueueService.Hangfire: // this is the default!
      default:
        s.AddTransient<IBackgroundJobEnqueueingService, HangfireQueueService>()
          .AddTransient<IRejectService, RejectService>()
          .AddTransient<ICommitService, CommitService>();
        break;
    }

    return s;
  }

  private static IServiceCollection AddDirectoryGuiServices(this IServiceCollection s)
  {
    return s.AddTransient<IAbstractCrudService, AbstractCrudService>()
      .AddTransient<IAnalyticsReportGenerator, AnalyticsReportGenerator>()
      .AddTransient<IBiobankService, BiobankService>()
      .AddTransient<IBiobankIndexService, BiobankIndexService>()
      .AddTransient<IBiobankAuthorizationService, BiobankAuthorizationService>()
      .AddTransient<ICapabilityService, CapabilityService>()
      .AddTransient<ICollectionService, CollectionService>()
      .AddTransient<IConfigService, ConfigService>()
      .AddTransient<IContentPageService, ContentPageService>()
      .AddTransient<ILogoStorageProvider, SqlServerLogoStorageProvider>()
      .AddTransient<ILogoService, LogoService>()
      .AddTransient<INetworkService, NetworkService>()
      .AddTransient<IOntologyTermService, OntologyTermService>()
      .AddTransient<IOrganisationDirectoryService,
        OrganisationDirectoryService>() //TODO: merge or resolve OrganisationDirectory and Organisation Services
      .AddTransient<IRecaptchaService, RecaptchaService>()
      .AddTransient<IRegistrationDomainService, RegistrationDomainService>()
      .AddTransient<ISampleSetService, SampleSetService>()
      .AddTransient<ITokenLoggingService, TokenLoggingService>();
  }

  private static IServiceCollection AddAggregatorServices(this IServiceCollection s)
  {
    return s.AddTransient<ISampleService, SampleService>()
      .AddTransient<IAggregationService, AggregationService>()
      .AddTransient<ICollectionAggregatorService, CollectionAggregatorService>()
      .AddTransient<IReferenceDataAggregatorService, ReferenceDataAggregatorService>();
  }

  private static IServiceCollection AddDirectoryAnalytics(this IServiceCollection s)
  {
    return s.AddTransient<IAnalyticsService, AnalyticsService>()
      .AddTransient<IDirectoryReportGenerator, DirectoryReportGenerator>()
      .AddTransient<IOrganisationReportGenerator, OrganisationReportGenerator>()
      .AddTransient<IReportDataTransformationService, ReportDataTransformationService>()
      .AddTransient<IGoogleAnalyticsReportingService, GoogleAnalyticsReportingService>();
  }

  private static IServiceCollection AddBiobankPublications(this IServiceCollection s)
  {
    return s.AddTransient<IPublicationJobService, PublicationJobService>()
      .AddTransient<IPublicationService, PublicationService>()
      .AddTransient<IAnnotationService, AnnotationService>()
      .AddTransient<IEpmcService, EpmcWebService>();
  }

  private static IServiceCollection AddSubmissionsApiServices(this IServiceCollection s)
  {
    return s.AddTransient<IErrorService, ErrorService>()
      .AddTransient<IDiagnosisWriteService, DiagnosisWriteService>()
      .AddTransient<IDiagnosisValidationService, DiagnosisValidationService>()
      .AddTransient<IReferenceDataReadService,
        ReferenceDataReadService>() // TODO: Merge ReferenceDataReadService and ReferenceDataService
      .AddTransient<ISampleWriteService, SampleWriteService>()
      .AddTransient<ISampleValidationService, SampleValidationService>()
      .AddTransient<ISubmissionExpiryService, SubmissionExpiryService>()
      .AddTransient<ISubmissionService, SubmissionService>()
      .AddTransient<ITreatmentWriteService, TreatmentWriteService>()
      .AddTransient<ITreatmentValidationService, TreatmentValidationService>();
  }
}
