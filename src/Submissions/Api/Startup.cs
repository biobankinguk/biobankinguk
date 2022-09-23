using Biobanks.IdentityModel.Helpers;
using Biobanks.Publications.Services;
using Biobanks.Publications.Services.Contracts;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Auth.Basic;
using Biobanks.Submissions.Api.Config;
using ClacksMiddleware.Extensions;
using Core.AzureStorage;

using Core.Jobs;
using Core.Submissions.Services;
using Core.Submissions.Services.Contracts;

using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerUI;

using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

using UoN.AspNetCore.VersionMiddleware;
using Biobanks.Shared.Services.Contracts;
using Biobanks.Shared.Services;
using Biobanks.Analytics.Services;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.Analytics;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Aggregator.Services;
using System.Linq;
using Microsoft.Extensions.Options;
using Hangfire.Dashboard;
using Biobanks.Submissions.Api.JsonConverters;
using Core.Submissions.Models.OptionsModels;
using Biobanks.Aggregator;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Services.Submissions.Contracts;
using Biobanks.Submissions.Api.Services.Submissions;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.Directory;

namespace Biobanks.Submissions.Api
{
    /// <summary>
    /// Main startup pipeline for app - configures services and middleware.
    /// </summary>
    public class Startup
    {
        /// <inheritdoc />
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Global configuration values.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the app's global services.
        /// </summary>
        /// <param name="services">Collection of services to be configured.</param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // local config
            var jwtConfig = Configuration.GetSection("JWT").Get<JwtBearerConfig>();
            var workersConfig = Configuration.GetSection("Workers").Get<WorkersOptions>() ?? new();
            var hangfireConfig = Configuration.GetSection("Hangfire").Get<HangfireOptions>() ?? new();

            // MVC
            services.AddControllers(opts => opts.SuppressOutputFormatterBuffering = true)
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    o.JsonSerializerOptions.Converters.Add(new JsonNumberAsStringConverter());
                });

            // Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

            services
                .AddOptions()

                .Configure<IISServerOptions>(opts => opts.AllowSynchronousIO = true)
                .Configure<JwtBearerConfig>(Configuration.GetSection("JWT"))
                .Configure<AggregatorOptions>(Configuration.GetSection("Aggregator"))
                .Configure<AnalyticsOptions>(Configuration.GetSection("Analytics"))
                .Configure<WorkersOptions>(Configuration.GetSection("Workers"))
                .Configure<HangfireOptions>(Configuration.GetSection("Hangfire"))

                .Configure<MaterialTypesLegacyModel>(Configuration.GetSection("MaterialTypesLegacyModel"))
                .Configure<StorageTemperatureLegacyModel>(Configuration.GetSection("StorageTemperatureLegacyModel"))

                .AddApplicationInsightsTelemetry()

                .AddDbContext<Data.BiobanksDbContext>(opts =>
                    opts.UseSqlServer(Configuration.GetConnectionString("Default"),
                        sqlServerOptions => sqlServerOptions.CommandTimeout(300000000)))

                .AddAuthorization(o =>
                {
                    o.DefaultPolicy = AuthPolicies.IsAuthenticated;
                    o.AddPolicy(nameof(AuthPolicies.IsTokenAuthenticated),
                        AuthPolicies.IsTokenAuthenticated);
                    o.AddPolicy(nameof(AuthPolicies.IsBasicAuthenticated),
                        AuthPolicies.IsBasicAuthenticated);
                    o.AddPolicy(nameof(AuthPolicies.IsSuperAdmin),
                        AuthPolicies.IsSuperAdmin);
                })

                .AddSwaggerGen(opts =>
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
                        opts.IncludeXmlComments(Path.Combine(
                            PlatformServices.Default.Application.ApplicationBasePath,
                            Configuration["Swagger:Filename"]));

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
                    })

                .AddAutoMapper(
                    typeof(Core.Submissions.MappingProfiles.DiagnosisProfile),
                    typeof(Startup))

                .AddHttpClient()

                .AddMemoryCache()

                // Cloud services
                .AddTransient<IBlobWriteService, AzureBlobWriteService>( // TODO: Merge Blob Read and Write services
                    _ => new(Configuration.GetConnectionString("AzureStorage")))
                .AddTransient<IBlobReadService, AzureBlobReadService>(
                    _ => new(Configuration.GetConnectionString("AzureStorage")))
                .AddTransient<IQueueWriteService, AzureQueueWriteService>(
                    _ => new(Configuration.GetConnectionString("AzureStorage")))

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


                .AddTransient<ICollectionService, CollectionService>()
                .AddTransient<ISampleService, SampleService>()
                .AddTransient<IOrganisationService, OrganisationService>()
                .AddTransient<IAggregationService, AggregationService>()
                .AddTransient(typeof(IReferenceDataService<>), typeof(ReferenceDataService<>))

                .AddTransient<IReferenceDataAggregatorService, ReferenceDataAggregatorService>()
                .AddTransient<ICollectionService, CollectionService>()
                .AddTransient<ISampleService, SampleService>()
                .AddTransient<IOrganisationService, OrganisationService>()
                .AddTransient<IAggregationService, AggregationService>()

                .AddTransient<IPublicationJobService, PublicationJobService>()
                .AddTransient<IAnnotationService, AnnotationService>()
                .AddTransient<IEpmcService, EpmcWebService>()
                .AddTransient<IOrganisationService, OrganisationService>()

                .AddTransient<IDirectoryReportGenerator, DirectoryReportGenerator>()
                .AddTransient<IOrganisationReportGenerator, OrganisationReportGenerator>()
                .AddTransient<IReportDataTransformationService, ReportDataTransformationService>()
                .AddTransient<IAnalyticsService, AnalyticsService>()
                .AddTransient<IGoogleAnalyticsReportingService, GoogleAnalyticsReportingService>()

                .AddTransient<ISubmissionExpiryService, SubmissionExpiryService>()

                //Directory Services
                .AddTransient<IPublicationService, PublicationService>();

            // Conditional services
            if (workersConfig.HangfireRecurringJobs.Any() || workersConfig.QueueService == WorkersQueueService.Hangfire)
            {
                var connectionString = Configuration.GetConnectionString("Hangfire");

                services.AddHangfire(x => x.UseSqlServerStorage(
                    !string.IsNullOrWhiteSpace(connectionString)
                        ? connectionString
                        : Configuration.GetConnectionString("Default"),
                    new() { SchemaName = hangfireConfig.SchemaName }));
            }

            switch (workersConfig.QueueService)
            {
                case WorkersQueueService.AzureQueueStorage:
                    services.AddTransient<IBackgroundJobEnqueueingService, AzureQueueService>();
                    break;
                // case WorkersQueueService.Hangfire: // this is the default!
                default:
                    services
                        .AddTransient<IBackgroundJobEnqueueingService, HangfireQueueService>()
                        .AddTransient<IRejectService, RejectService>()
                        .AddTransient<ICommitService, CommitService>();
                    break;
            }
        }

        /// <summary>
        /// Configures the main HTTP pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="workersOptions"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IOptions<WorkersOptions> workersOptions)
        {
            // Early pipeline config
            app
                .GnuTerryPratchett()
                .UseHttpsRedirection();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

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

                // Everything past this point is routed and subject to Auth
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()

                // Endpoint Routing
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers().RequireAuthorization();

                    if (env.IsDevelopment())
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
                .UseHangfireServer();

            // App Startup tasks
            ConfigureHangfireRecurringJobs(workersOptions.Value);
        }

        private void ConfigureHangfireRecurringJobs(WorkersOptions workersConfig)
        {
            // Hangfire Recurring Jobs, as configured
            Dictionary<string, Action> jobs = new()
            {
                [WorkersRecurringJobs.Analytics] = ()
                    => RecurringJob.AddOrUpdate<AnalyticsJob>("job-analytics", x => x.Run(), "0 0 1 */3 *"),

                [WorkersRecurringJobs.Publications] = ()
                    => RecurringJob.AddOrUpdate<PublicationsJob>("job-publications", x => x.Run(), Cron.Daily()),

                [WorkersRecurringJobs.Aggregator] = ()
                    => RecurringJob.AddOrUpdate<AggregatorJob>("job-aggregator", x => x.Run(), Cron.Daily()),

                [WorkersRecurringJobs.SubmissionsExpiry] = ()
                    => RecurringJob.AddOrUpdate<ExpiryJob>("job-expiry", x => x.Run(), Cron.Daily())
            };

            // run each job the config opts us in to
            foreach (var job in workersConfig.HangfireRecurringJobs)
                if (jobs.ContainsKey(job)) jobs[job]();
        }
    }
}