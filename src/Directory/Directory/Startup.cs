﻿using AutoMapper;
using ClacksMiddleware.Extensions;
using Common.Constants;
using Common.Data;
using Common.MappingProfiles;
using Directory.Auth;
using Directory.Contracts;
using Directory.IdentityServer;
using Directory.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Directory
{
    public class Startup
    {
        public readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Identity Server
            services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients(_config))
                .AddTestUsers(Config.GetUsers())
                .AddDeveloperSigningCredential(); // TODO: Configure non-dev signing

            // MVC
            services.AddControllers();
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            // Auth
            services.AddAuthentication() // DO NOT set a default; IdentityServer does that
                // Also add Bearer Auth for our API
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                {
                    opts.Authority = _config["JwtBearer:Authority"];
                    opts.Audience = ApiResourceKeys.RefData;
                });
            services.AddAuthorization(opts =>
                {
                    opts.AddPolicy(nameof(AuthPolicies.BearerToken), AuthPolicies.BearerToken);
                });

            // Entity Framework
            services.AddDbContext<DirectoryContext>(opts => opts
                .UseLazyLoadingProxies()
                .UseSqlServer(_config.GetConnectionString("DefaultConnection")));

            //service layer
            services.AddTransient<IReferenceDataReadService, ReferenceDataReadService>();
            services.AddTransient<IReferenceDataWriterService, ReferenceDataWriterService>();

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Biobanks Directory API", Version = "v1" });
                c.EnableAnnotations();
            });

            //Other third party
            services.AddAutoMapper(typeof(RefDataBaseDtoProfile));
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

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers().RequireAuthorization(nameof(AuthPolicies.BearerToken));
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Biobanks API v1");
            });
        }
    }
}
