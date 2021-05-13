using Biobanks.IdentityModel.Helpers;
﻿using Biobanks.Publications.Services;
using Biobanks.Publications.Services.Contracts;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Auth.Basic;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Services;
using Biobanks.Submissions.Api.Services.Contracts;
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
        /// <returns></returns>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // local config
            var jwtConfig = Configuration.GetSection("JWT").Get<JwtBearerConfig>();

            // MVC
            services.AddControllers(opts => opts.SuppressOutputFormatterBuffering = true)
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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
                .AddBasic(opts => opts.Realm = "biobankinguk-submissions-accesstoken");

            services
                .AddOptions()

                .Configure<IISServerOptions>(opts => opts.AllowSynchronousIO = true)
                .Configure<JwtBearerConfig>(Configuration.GetSection("JWT"))

                .AddApplicationInsightsTelemetry()

                .AddDbContext<Data.BiobanksDbContext>(opts =>
                    opts.UseSqlServer(Configuration.GetConnectionString("Default"),
                        sqlServerOptions => sqlServerOptions.CommandTimeout(300000000)))

                .AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("Default"),
                    new Hangfire.SqlServer.SqlServerStorageOptions
                    {
                        SchemaName = "apiHangfire"
                    }))

                .AddAuthorization(o =>
                {
                    o.DefaultPolicy = AuthPolicies.IsTokenAuthenticated;
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
                                Title = "BiobankingUK Submissions API",
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
                    })

                .AddAutoMapper(
                    typeof(Core.Submissions.MappingProfiles.DiagnosisProfile),
                    typeof(Startup))

                .AddHttpClient()

                // Cloud services
                .AddTransient<IBlobWriteService, AzureBlobWriteService>(
                    _ => new(Configuration.GetConnectionString("AzureStorage")))
                .AddTransient<IQueueWriteService, AzureQueueWriteService>(
                    _ => new(Configuration.GetConnectionString("AzureStorage")))

                // Local Services
                .AddTransient<ISubmissionService, SubmissionService>()
                .AddTransient<IErrorService, ErrorService>()

                .AddTransient<IPublicationService, PublicationService>()
                .AddTransient<IAnnotationService, AnnotationService>()
                .AddTransient<IEpmcService, EpmcWebService>()
                .AddTransient<IOrganisationService, OrganisationService>()
                .AddTransient<IGoogleAnalyticsReadService, GoogleAnalyticsReadService>()

                //Conditional Service (todo setup hangfire specific DI)
                .AddTransient<IBackgroundJobEnqueueingService, AzureQueueService>();

            //TODO Register these services if we're using hangfire
            //.AddTransient<IRejectService, RejectService>()
            //.AddTransient<ICommitService, CommitService>()
        }

        /// <summary>
        /// Configures the main HTTP pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

                    endpoints
                        .MapHangfireDashboard("/hangfire")
                        .RequireAuthorization(nameof(AuthPolicies.IsSuperAdmin));
                })

                // Hangfire Server
                .UseHangfireServer();

            // Hangfire Recurring Jobs
            RecurringJob.AddOrUpdate<AnalyticsJob>("job-analytics", x => x.Run(), "0 0 1 */3 *");
            RecurringJob.AddOrUpdate<PublicationsJob>("job-publications", x => x.Run(), Cron.Daily());
            RecurringJob.AddOrUpdate<ExpiryJob>("job-expiry", x => x.Run(), Cron.Daily);
        }
    }
}
