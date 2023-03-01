using System;
using System.Collections.Generic;
using Biobanks.Submissions.Api.Auth;
using ClacksMiddleware.Extensions;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using UoN.AspNetCore.VersionMiddleware;

namespace Biobanks.Submissions.Api.Startup.Web;

public static class ConfigureWebPipeline
{
  /// <summary>
  /// Configure the HTTP Request Pipeline for an ASP.NET Core app
  /// </summary>
  /// <param name="app"></param>
  /// <returns></returns>
  public static WebApplication UseWebPipeline(this WebApplication app)
  {
    app.GnuTerryPratchett();

    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }
    else
    {
      app.UseExceptionHandler("/Error");
      // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
    app.UseStaticFiles();

    app
      // Simple PUBLIC middleware
      .UseVersion()

      // Swagger
      .UseSwagger(c =>
      {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
          swaggerDoc.Servers = new List<OpenApiServer>
          {
            new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
          });
      })
      .UseSwaggerUI(c =>
      {
        c.RoutePrefix = "swagger";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        c.SupportedSubmitMethods(SubmitMethod.Get);
      });

    // Routing, Auth etc BEFORE endpoints / private middleware
    app.UseRouting()
      .UseAuthentication()
      .UseAuthorization()
      .UseSession();

    // Map Endpoints
    app.UseAndMapHangfireDashboard();
    
    // MVC
    
    // API Controllers
    app.MapControllers().RequireAuthorization();
    
    // Razor Pages (e.g. Status errors)
    app.MapRazorPages();

    // MVC Controllers
    app.MapAreaControllerRoute(
      name: "AdminArea",
      areaName: "Admin",
      pattern: "Admin/{controller=Home}/{action=Index}/{id?}");       

    app.MapAreaControllerRoute(
      name: "BiobankArea",
      areaName: "Biobank",
      pattern: "Biobank/{controller=Home}/{action=Index}/{biobankId?}/{id?}");    

    app.MapAreaControllerRoute(
      name: "NetworkArea",
      areaName: "Network",
      pattern: "Network/{controller=Home}/{action=Index}/{networkId?}/{id?}");

    app.MapControllerRoute(
      name: "ContentPage",
      pattern: "Pages/{slug}",
      defaults: new { controller = "PagesAdmin", action = "ContentPage", Area= "Admin"});

    app.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");

    return app;
  }

  private static WebApplication UseAndMapHangfireDashboard(this WebApplication app)
  {
    // In Development, Map the dashboard without any auth
    if (app.Environment.IsDevelopment())
    {
      app.MapHangfireDashboard("/hangfire");
    }
    else // Otherwise require authentication
    {
      var dashboardOptions = new DashboardOptions()
      {
        Authorization = Array.Empty<IDashboardAuthorizationFilter>() // Removes Default Local-Auth Filter
      };

      app
        .MapHangfireDashboard("/hangfire", dashboardOptions)
        .RequireAuthorization(nameof(AuthPolicies.CanAccessHangfireDashboard));
    }

    // Add the middleware
    app.UseHangfireDashboard();

    return app;
  }
}
