using AutoMapper;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
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
            var defaultDb = _config.GetConnectionString("DefaultConnection");

            // ASP.NET Core Identity
            services.AddIdentityCore<IdentityUser>() // TODO: replace with custom user
                .AddEntityFrameworkStores<DirectoryContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<IdentityUser>>();

            // Identity Server
            services.AddIdentityServer()
                .AddTestUsers(TemporaryConfig.GetUsers())
                .AddConfigurationStore<DirectoryContext>(opts =>
                    opts.ConfigureDbContext = b => b.UseSqlServer(defaultDb))
                .AddOperationalStore<DirectoryContext>(opts =>
                {
                    opts.ConfigureDbContext = b => b.UseSqlServer(defaultDb);
                    opts.EnableTokenCleanup = true;
                })
                .AddDeveloperSigningCredential(); // TODO: Configure non-dev signing

            // MVC
            services.AddControllers();
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();
            // ClientApp
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            // Auth
            services.AddAuthentication() // DO NOT set a default; IdentityServer does that
                // Also add Bearer Auth for our API
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                {
                    opts.Authority = _config["JwtBearer:Authority"];
                    opts.Audience = ApiResourceKeys.RefData;
                });
            services.AddAuthorization(opts =>
                opts.AddPolicy(nameof(AuthPolicies.BearerToken), AuthPolicies.BearerToken));

            // Entity Framework
            services.AddDbContext<DirectoryContext>(opts => opts
                .UseLazyLoadingProxies()
                .UseSqlServer(defaultDb));

            // Service layer
            services.AddTransient<IReferenceDataReadService, ReferenceDataReadService>();
            services.AddTransient<IReferenceDataWriterService, ReferenceDataWriterService>();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UKCRC Tissue Directory API", Version = "v1" });
                c.EnableAnnotations();
            });

            // Other third party
            services.AddAutoMapper(typeof(RefDataBaseDtoProfile));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // App initialisation
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DirectoryContext>();
                context.Database.Migrate();

                IdentityServer.DataSeeder.Seed(context, _config);
            }

            // Pipeline Configuration
            app.GnuTerryPratchett();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers().RequireAuthorization(nameof(AuthPolicies.BearerToken));
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "UKCRC Tissue Directory API"));

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                    spa.UseReactDevelopmentServer(npmScript: "start");
            });
        }
    }
}
