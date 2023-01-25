using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Biobanks.Aggregator;
using Biobanks.Aggregator.Services;
using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Analytics;
using Biobanks.Analytics.Services;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Omop.Context;
using Biobanks.Publications.Services;
using Biobanks.Publications.Services.Contracts;
using Biobanks.Search.Contracts;
using Biobanks.Search.Elastic;
using Biobanks.Search.Legacy;
using Biobanks.Shared.Services;
using Biobanks.Shared.Services.Contracts;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Auth.Basic;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Extensions;
using Biobanks.Submissions.Api.Filters;
using Biobanks.Submissions.Api.JsonConverters;
using Biobanks.Submissions.Api.Middleware;
using Biobanks.Submissions.Api.Services;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.Submissions;
using Biobanks.Submissions.Api.Services.Submissions.Contracts;
using Biobanks.Submissions.Api.Utilities.IdentityModel;
using ClacksMiddleware.Extensions;
using cloudscribe.Web.SiteMap;
using Core.AzureStorage;
using Core.Submissions.Models.OptionsModels;
using Core.Submissions.Services;
using Core.Submissions.Services.Contracts;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using UoN.AspNetCore.VersionMiddleware;

namespace Biobanks.Submissions.Api.Startup.Web;

public static class WebEntrypoint
{
  public static async Task Run(string[] args)
  {
    var b = WebApplication.CreateBuilder(args);

    // Extra Configuration Loading
    b.Configuration
      .AddJsonFile("Settings/LegacyMaterialTypes.json", optional: true)
      .AddJsonFile("Settings/LegacyStorageTemperatures.json", optional: true);

    // Configure DI Services
    b.ConfigureServices();

    // Build the app
    var app = b.Build();
    
    // Initialisation before the app runs
    await app.Initialise();

    // Configure the HTTP Request Pipeline
    app.UseWebPipeline();

    // Run the app!
    await app.RunAsync();
  }
}
