using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Filters;
using Biobanks.Submissions.Api.Services;
using Biobanks.Submissions.Api.Services.Contracts;
using Biobanks.Submissions.Core.AzureStorage;
using Biobanks.Submissions.Core.Services;
using Biobanks.Submissions.Core.Services.Contracts;

using clacks.overhead;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
            services.AddApplicationInsightsTelemetry();

            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("Default")));

            services.AddDbContext<Data.BiobanksDbContext>(opts =>
                opts.UseSqlServer(Configuration.GetConnectionString("Default"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(300000000)));

            services.AddMvc(opts =>
            {
                opts.Filters.Add(new AuthorizeFilter(AuthPolicies.BuildDefaultJwtPolicy()));
                opts.EnableEndpointRouting = false;
            })
                // TODO: Consider System.Text.Json
                .AddNewtonsoftJson(opts =>
                    {
                        opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                        opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                    });

            // JWT Auth
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
                });

            services.AddSwaggerGen(opts =>
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
            });
            services.AddSwaggerGenNewtonsoftSupport();

            // Add Core Mapping Profiles, and any Local ones
            services.AddAutoMapper(
                typeof(Core.MappingProfiles.DiagnosisProfile),
                typeof(Startup));

            // Synchronous I/O is disabled by default in .NET Core 3
            services.Configure<IISServerOptions>(opts =>
            {
                opts.AllowSynchronousIO = true;
            });

            // disable output format buffering
            services.AddControllers(opts => opts.SuppressOutputFormatterBuffering = true);


            // Cloud Services
            services.AddTransient<IBlobWriteService, AzureBlobWriteService>(
                _ => new(Configuration.GetConnectionString("AzureStorage")));
            services.AddTransient<IQueueWriteService, AzureQueueWriteService>(
                _ => new(Configuration.GetConnectionString("AzureStorage")));


            services.AddTransient<ISubmissionService, SubmissionService>();
            services.AddTransient<IErrorService, ErrorService>();
            services.AddTransient<ICommitService, CommitService>();
            services.AddTransient<IRejectService, RejectService>();

            // reference data services
            services.AddTransient<IMaterialTypeService, MaterialTypeService>();
            services.AddTransient<IMaterialTypeGroupService, MaterialTypeGroupService>();
            services.AddTransient<IOntologyService, OntologyService>();
            services.AddTransient<IOntologyVersionService, OntologyVersionService>();
            services.AddTransient<ISampleContentMethodService, SampleContentMethodService>();
            services.AddTransient<ISexService, SexService>();
            services.AddTransient<ISnomedTagService, SnomedTagService>();
            services.AddTransient<IOntologyTermService, OntologyTermService>();
            services.AddTransient<IStorageTemperatureService, StorageTemperatureService>();
            services.AddTransient<ITreatmentLocationService, TreatmentLocationService>();


            services.AddOptions();
        }

        /// <summary>
        /// Configures the main HTTP pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.RememberTerryPratchett(); // TODO: replace with newer Clacks middleware

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // TODO only enable when ready
                //app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStatusCodePages();

            app.UseVersion();
            /*
            app.UseSwagger(c =>
                c.PreSerializeFilters.Add((swagger, request) =>
                    swagger.Host = request.Host.Value));
            */

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                });
            });



            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty; // serve swagger ui from root ;)
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1 Docs");
                c.SupportedSubmitMethods(); // don't allow "try it out" as the token auth doesn't work
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Hangfire
            app.UseHangfireServer();
            app.UseHangfireDashboard("/TasksDashboard", new DashboardOptions
            {
                Authorization = new [] {new HangfireDashboardAuthorizationFilter()}
            });
        }
    }
}
