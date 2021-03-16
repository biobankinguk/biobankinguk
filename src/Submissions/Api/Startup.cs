using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Filters;
using Biobanks.Submissions.Api.Services;
using Biobanks.Submissions.Api.Services.Contracts;
using Biobanks.Submissions.Core.AzureStorage;
using Biobanks.Submissions.Core.Services;
using Biobanks.Submissions.Core.Services.Contracts;

using ClacksMiddleware.Extensions;

using Hangfire;

using Microsoft.AspNetCore.Authentication;
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

using UoN.AspNetCore.VersionMiddleware;

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
                    opts.Audience = Configuration["JWT:Audience"];
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Convert.FromBase64String(
                                Configuration["JWT:Secret"])),
                        RequireExpirationTime = false
                    };
                })
                .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>(BasicAuthConstants.AuthenticationScheme, null);

            services
                .AddOptions()

                .Configure<IISServerOptions>(opts => opts.AllowSynchronousIO = true)

                .AddApplicationInsightsTelemetry()

                .AddDbContext<Data.BiobanksDbContext>(opts =>
                    opts.UseSqlServer(Configuration.GetConnectionString("Default"),
                        sqlServerOptions => sqlServerOptions.CommandTimeout(300000000)))

                .AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("Default")))

                .AddAuthorization(o =>
                {
                    o.DefaultPolicy = AuthPolicies.IsTokenAuthenticated;
                    o.AddPolicy(nameof(AuthPolicies.IsBasicAuthenticated),
                        AuthPolicies.IsBasicAuthenticated);
                })

                .AddSwaggerGen(opts =>
                    {
                        opts.SwaggerDoc("v1",
                            new OpenApiInfo
                            {
                                Title = "UKCRC Tissue Directory API",
                                Version = "v1"
                            });

                        opts.IncludeXmlComments(Path.Combine(
                            PlatformServices.Default.Application.ApplicationBasePath,
                            Configuration["Swagger:Filename"]));
                    })

                .AddAutoMapper(
                    typeof(Core.MappingProfiles.DiagnosisProfile),
                    typeof(Startup))

                // Cloud services
                .AddTransient<IBlobWriteService, AzureBlobWriteService>(
                    _ => new(Configuration.GetConnectionString("AzureStorage")))
                .AddTransient<IQueueWriteService, AzureQueueWriteService>(
                    _ => new(Configuration.GetConnectionString("AzureStorage")))

                // Local Services
                .AddTransient<ISubmissionService, SubmissionService>()
                .AddTransient<IErrorService, ErrorService>()
                .AddTransient<ICommitService, CommitService>()
                .AddTransient<IRejectService, RejectService>();
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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1 Docs");
                    c.SupportedSubmitMethods(); // don't allow "try it out" as the token auth doesn't work
                })

                // Everything past this point is routed and subject to Auth
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()

                // Endpoint Routing
                .UseEndpoints(endpoints => endpoints.MapControllers().RequireAuthorization())

                // Hangfire
                .UseHangfireServer()
                .UseHangfireDashboard("/TasksDashboard", new DashboardOptions
                {
                    Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
                });
        }
    }
}
