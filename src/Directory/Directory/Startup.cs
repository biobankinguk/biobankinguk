using ClacksMiddleware.Extensions;
using Common.Constants;
using Common.Data;
using Directory.Contracts;
using Directory.IdentityServer;
using Directory.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients(_config))
                .AddTestUsers(Config.GetUsers())
                .AddDeveloperSigningCredential(); // TODO: Configure non-dev signing

            services.AddControllers();
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            //    {
            //        opts.Authority = _config["JwtBearer:Authority"];
            //        opts.Audience = ApiResourceKeys.RefData;
            //    })
            //    .AddCookie();

            // Entity Framework
            services.AddDbContext<DirectoryContext>(opts => opts
                .UseLazyLoadingProxies()
                .UseSqlServer(_config.GetConnectionString("DefaultConnection")));

            //service layer
            services.AddTransient<IReferenceDataReadService, ReferenceDataReadService>();
            services.AddTransient<IReferenceDataWriterService, ReferenceDataWriterService>();
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
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}
