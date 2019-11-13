using System;
using System.Threading.Tasks;
using AutoMapper;
using ClacksMiddleware.Extensions;
using Common.Constants;
using Common.Data;
using Common.Data.Identity;
using Common.MappingProfiles;
using Directory.Auth;
using Directory.Auth.Identity;
using Directory.Config;
using Directory.Contracts;
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
using UoN.AspNetCore.RazorViewRenderer;
namespace Directory
{
    public class Startup
    {
        public readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Called by the WebHostBuilder to Register DI Services.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            var defaultDb = _config.GetConnectionString("DefaultConnection");
            var useSendGrid = !string.IsNullOrEmpty(_config["OutboundEmail:SendGridApiKey"]);

            services.AddResponseCompression();

            // ASP.NET Core Identity
            services.AddIdentityCore<DirectoryUser>()
                .AddEntityFrameworkStores<DirectoryContext>()
                .AddDefaultTokenProviders()
                .AddUserManager<DirectoryUserManager>()
                .AddSignInManager<SignInManager<DirectoryUser>>();

            // Identity Server
            services.AddIdentityServer()
                .AddConfigurationStore<DirectoryContext>(opts =>
                    opts.ConfigureDbContext = b => b.UseSqlServer(defaultDb))
                .AddOperationalStore<DirectoryContext>(opts =>
                {
                    opts.ConfigureDbContext = b => b.UseSqlServer(defaultDb);
                    opts.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<DirectoryUser>()
                .AddDeveloperSigningCredential(); // TODO: Configure non-dev signing

            // MVC
            services.AddControllers();
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();
            // ClientApp
            services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/build");

            // Auth
            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                {
                    opts.Authority = _config["JwtBearer:Authority"];
                    opts.Audience = ApiResourceKeys.RefData;
                })
                .AddIdentityCookies();
            services.AddAuthorization(opts =>
                opts.AddPolicy(nameof(AuthPolicies.BearerToken), AuthPolicies.BearerToken));

            // Entity Framework
            services.AddDbContext<DirectoryContext>(opts => opts
                .UseLazyLoadingProxies()
                .UseSqlServer(defaultDb));

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UKCRC Tissue Directory API", Version = "v1" });
                c.EnableAnnotations();
            });

            // Other third party
            services.AddAutoMapper(typeof(RefDataBaseDtoProfile));

            // Service layer
            services.AddTransient<IReferenceDataReadService, ReferenceDataReadService>();
            services.AddTransient<IReferenceDataWriterService, ReferenceDataWriterService>();
            services.AddTransient<IRazorViewRenderer, RazorViewRenderer>();
            services.AddTransient<AccountEmailService>();
            services.AddTransient<TokenLoggingService>();

            if (useSendGrid) services.AddTransient<IEmailSender, SendGridEmailSender>();
            else services.AddTransient<IEmailSender, LocalDiskEmailSender>();

            // Configuration
            services.Configure<IdentityOptions>(_config.GetSection("Identity"))
                .Configure<DataProtectionTokenProviderOptions>(options =>
                    options.TokenLifespan = TimeSpan.FromDays(5));

            if (useSendGrid) services.Configure<SendGridOptions>(_config.GetSection("OutboundEmail"));
            else services.Configure<LocalMailOptions>(_config.GetSection("OutboundEmail"));
        }

        /// <summary>
        /// Called by the WebHostBuilder to to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

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

        /// <summary>
        /// Called in Main to peform any App Initialisation
        /// before the WebHost itself is run.
        /// </summary>
        public static async Task Initialise(IServiceProvider services)
        {
            var config = services.GetRequiredService<IConfiguration>();
            var context = services.GetRequiredService<DirectoryContext>();

            context.Database.Migrate();

            Auth.IdentityServer.DataSeeder.Seed(context, config);

            await Auth.Identity.DataSeeder.Seed(
                services.GetRequiredService<DirectoryUserManager>(),
                services.GetRequiredService<IPasswordHasher<DirectoryUser>>(),
                config);
        }
    }
}
