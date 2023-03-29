using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Submissions.Api.Services.DataSeeding;
using Biobanks.Submissions.Api.Services.Directory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Biobanks.Submissions.Api.Startup.Web;

public static class WebInitialisation
{
  /// <summary>
  ///  Do any app initialisation after the the app has been built (and DI services are locked down)
  ///
  /// e.g. Internal Data Seeding on App Startup
  /// </summary>
  /// <param name="app"></param>
  public static async Task Initialise(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
      .GetRequiredService<ApplicationDbContext>();
    var roles = scope.ServiceProvider
      .GetRequiredService<RoleManager<IdentityRole>>();

    // The Web app always tries to ensure fixed ref data is correct
    await new FixedRefDataSeeder(db, roles).Seed();

    // Set cache isolated from running of the app
    var configCache = scope.ServiceProvider
      .GetRequiredService<IConfigService>();
    await configCache.PopulateSiteConfigCache();
  }

  /// <summary>
  /// Do any app initialisation that requires the host builder (before the app is built)
  ///
  /// This is less commonly required
  /// </summary>
  /// <param name="b"></param>
  public static async Task Initialise(this WebApplicationBuilder b)
  {
    // Extra Configuration Loading
    b.Configuration
      .AddJsonFile("Settings/LegacyMaterialTypes.json", optional: true)
      .AddJsonFile("Settings/LegacyStorageTemperatures.json", optional: true);

    await Task.CompletedTask;
  }
}
