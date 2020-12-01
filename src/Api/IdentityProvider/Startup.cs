using System;
using Biobanks.IdentityProvider.Data;
using clacks.overhead;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UoN.AspNetCore.VersionMiddleware;

namespace Biobanks.IdentityProvider
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IdpDbContext>(
                opts => opts.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdpDbContext>();

            const string authScheme = "BasicAuthentication";
            services.AddAuthentication(authScheme)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>(
                authScheme, null);

            // anything that needs our configuration injecting
            // should get the property instance from here
            services.AddSingleton(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            // first migrate the database
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<IdpDbContext>())
                {
                    context.Database.Migrate();
                }
            }

            SeedUsers(services); // TODO do this in a more net core 2 style way?

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

            app.UseStaticFiles();

            app.UseVersion();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty; // serve swagger ui from root ;)
                c.SwaggerEndpoint("/swagger-v1.json", "v1 Docs");
            });

            // handle /identity only
            app.Map("/identity", map =>
            {
                map.UseAuthentication();
                map.UseMiddleware<IdentityJwtProviderMiddleware>();
            });
        }
    }
}
