using System.IdentityModel.Tokens.Jwt;
using Common.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MvcPoc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(opts =>
            {
                opts.DefaultScheme = "Cookies"; // can we not use constants for this is4?!
                opts.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            // Example Hybrid Flow Configuration
            // This should be used if the Client doesn't support PKCE
            // ASP.NET Core does, so really this app shouldn't use this
            // But the Framework Directory may not support it, so this serves as an example.
            //
            //.AddOpenIdConnect("oidc", opts =>
            //{
            //    opts.SignInScheme = "Cookies";

            //    opts.Authority = "https://localhost:5001";
            //    opts.RequireHttpsMetadata = true;

            //    opts.ClientId = "mvc-poc-hybrid";
            //    opts.ClientSecret = Configuration[$"ClientSecrets:{TrustedClientIds.UploadApi}"];
            //    opts.ResponseType = "code id_token";

            //    opts.SaveTokens = true;
            //    opts.GetClaimsFromUserInfoEndpoint = true;

            //    opts.Scope.Add(ApiResourceKeys.RefData);
            //    opts.Scope.Add("offline_access");
            //    //opts.ClaimActions.MapJsonKey("website", "website"); // TODO: actually we don't have this claim...
            //});
            //
            // Example PKCE Flow Configuration
            // This should always be used if the Client supports PKCE, which ASP.NET Core does
            // But the Framework Directory may not support it, so the above Hybrid Config may be needed.
            //
            .AddOpenIdConnect("oidc", opts =>
            {
                opts.SignInScheme = "Cookies";

                opts.Authority = "https://localhost:5001";
                opts.RequireHttpsMetadata = true;

                opts.ClientId = "mvc-poc-pkce";
                opts.ClientSecret = Configuration[$"ClientSecrets:{TrustedClientIds.UploadApi}"];
                opts.ResponseType = "code";

                opts.SaveTokens = true;
                opts.GetClaimsFromUserInfoEndpoint = true;

                opts.Scope.Add(ApiResourceKeys.RefData);
                opts.Scope.Add("offline_access");
                //opts.ClaimActions.MapJsonKey("website", "website"); // TODO: actually we don't have this claim...
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
