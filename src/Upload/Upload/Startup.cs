using AutoMapper;
using Biobanks.SubmissionApi.Services.Contracts;
using ClacksMiddleware.Extensions;
using Common.Constants;
using Common.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Threading.Tasks;
using Upload.Azure;
using Upload.Config;
using Upload.Contracts;
using Upload.Profiles;
using Upload.Services;

namespace Upload
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var defaultDb = _config.GetConnectionString("DefaultConnection");

            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", false, true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
               .AddEnvironmentVariables()
               .Build();
            services.AddSingleton(_ => config);
            services.Configure<ApiSettings>(config.GetSection("ApiSettings"));

            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                opts =>
                {
                    opts.Authority = _config["JwtBearer:Authority"];
                    opts.Audience = ApiResourceKeys.Upload;
                });

            // Entity Framework
            services.AddDbContext<UploadContext>(opts => opts
                .UseLazyLoadingProxies()
                .UseSqlServer(defaultDb));

            //
            services.AddTransient(s =>
                        CloudStorageAccount.Parse(
                            _config.GetConnectionString("blobstorage")));

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UKCRC Tissue Directory Upload API", Version = "v1" });
                c.EnableAnnotations();
            });

            //Other 3rd party
            services.AddAutoMapper(typeof(DiagnosisProfile));

            //services
            services.AddTransient<ICommitService, CommitService>();
            services.AddTransient<IErrorService, ErrorService>();
            services.AddTransient<IRejectService, RejectService>();
            services.AddTransient<ISubmissionService, SubmissionService>();

            //Services which talk to Azure. Can be swapped out for other services
            //TODO replace with an extension method which handles DI for Azure (bit neater)
            services.AddTransient<IBlobWriteService, AzureBlobWriteService>();
            services.AddTransient<IQueueWriteService, AzureQueueWriteService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.GnuTerryPratchett();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "UKCRC Tissue Director Upload API"));

            
        }
    }
}
