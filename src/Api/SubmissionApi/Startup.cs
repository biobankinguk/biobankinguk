using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Biobanks.Common.Auth;
using Biobanks.Common.Data;
using Biobanks.SubmissionApi.Filters;
using Biobanks.SubmissionApi.Services;
using Biobanks.SubmissionApi.Services.Contracts;
using clacks.overhead;
using Hangfire;
using Hangfire.Dashboard;
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
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using UoN.AspNetCore.VersionMiddleware;

namespace Biobanks.SubmissionApi
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
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<SubmissionsDbContext>(opts =>
                opts.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(300000000)));

            services.AddMvc(opts =>
            {
                opts.Filters.Add(new AuthorizeFilter(AuthPolicies.BuildDefaultJwtPolicy()));
                opts.EnableEndpointRouting = false;
            })
                .AddNewtonsoftJson(opts =>
                    opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);

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
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "UKCRC Tissue Directory API",
                        Version = "v1"
                    });

                opts.IncludeXmlComments(Path.Combine(
                    PlatformServices.Default.Application.ApplicationBasePath,
                    Configuration["Swagger:Filename"]));

                opts.DescribeAllEnumsAsStrings();
            });

            services.AddAutoMapper();

            // Synchronous I/O is disabled by default in .NET Core 3
            services.Configure<IISServerOptions>(opts =>
            {
                opts.AllowSynchronousIO = true;
            });

            // disable output fortmat buffering
            services.AddControllers(opts => opts.SuppressOutputFormatterBuffering = true);
            
            services.AddTransient<ISubmissionService, SubmissionService>();
            services.AddTransient<IErrorService, ErrorService>();
            services.AddTransient<IBlobWriteService, AzureBlobWriteService>();
            services.AddTransient<IQueueWriteService, AzureQueueWriteService>();
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
            services.AddTransient<ISnomedTermService, SnomedTermService>();
            services.AddTransient<IStorageTemperatureService, StorageTemperatureService>();
            services.AddTransient<ITreatmentLocationService, TreatmentLocationService>();

            //autofac.Populate(services); //load the basic services into autofac's container
            //return new AutofacServiceProvider(autofac.Build());
            services.AddOptions();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.Register(
                    (c, p) => CloudStorageAccount.Parse(Configuration.GetConnectionString("AzureQueueConnectionString")))
                .AsSelf();
        }

        /// <summary>
        /// Configures the main HTTP pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // first migrate the database
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<SubmissionsDbContext>())
                {
                    context.Database.Migrate();
                }
            }

            app.RememberTerryPratchett();

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

            app.UseSwagger(c =>
                c.PreSerializeFilters.Add((swagger, request) =>
                    swagger.Host = request.Host.Value));

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
