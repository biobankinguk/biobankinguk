using Biobanks.Aggregator;
using Biobanks.Analytics;
using Biobanks.Data;
using Biobanks.IdentityModel.Helpers;
using Biobanks.Omop.Context;
using Core.Submissions.Models.OptionsModels;
using Directory.Auth;
using Directory.Auth.Basic;
using Directory.Config;
using Directory.JsonConverters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//databases
builder.Services.AddDbContext<OmopDbContext>(options =>
options.UseNpgsql("Omop"));

builder.Services.AddDbContext<BiobanksDbContext>(options =>
    options.UseSqlServer(connectionString, 
    sqlServerOptions => sqlServerOptions.CommandTimeout(300000000)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<BiobanksDbContext>();

builder.Services.AddRazorPages();

builder.Configuration.AddJsonFile("Settings/LegacyMaterialTypes.json", optional: true);
builder.Configuration.AddJsonFile("Settings/LegacyStorageTemperatures.json", optional: true);

// local config
var jwtConfig = builder.Configuration.GetSection("JWT").Get<JwtBearerConfig>();
var workersConfig = builder.Configuration.GetSection("Workers").Get<WorkersOptions>() ?? new();
var hangfireConfig = builder.Configuration.GetSection("Hangfire").Get<HangfireOptions>() ?? new();

builder.Services.AddOptions()
    .Configure<IISServerOptions>(opts => opts.AllowSynchronousIO = true)
                .Configure<JwtBearerConfig>(builder.Configuration.GetSection("JWT"))
                .Configure<AggregatorOptions>(builder.Configuration.GetSection("Aggregator"))
                .Configure<AnalyticsOptions>(builder.Configuration.GetSection("Analytics"))
                .Configure<WorkersOptions>(builder.Configuration.GetSection("Workers"))
                .Configure<HangfireOptions>(builder.Configuration.GetSection("Hangfire"))
                .Configure<MaterialTypesLegacyModel>(builder.Configuration.GetSection("MaterialTypesLegacyModel"))
                .Configure<StorageTemperatureLegacyModel>(builder.Configuration.GetSection("StorageTemperatureLegacyModel"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.Audience = JwtBearerConstants.TokenAudience;
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = JwtBearerConstants.TokenIssuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = Crypto.GenerateSigningKey(jwtConfig.Secret),
                        RequireExpirationTime = true
                    };
                })
                .AddBasic(opts => opts.Realm = "biobankinguk-api");

builder.Services.AddControllersWithViews(opts => opts.SuppressOutputFormatterBuffering = true)
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    o.JsonSerializerOptions.Converters.Add(new JsonNumberAsStringConverter());
                });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
