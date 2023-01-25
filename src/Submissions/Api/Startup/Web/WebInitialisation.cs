using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Submissions.Api.Services.DataSeeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
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
    var db = app.Services.GetRequiredService<ApplicationDbContext>();
    var roles = app.Services.GetRequiredService<RoleManager<IdentityRole>>();

    // The Web app always tries to ensure fixed ref data is correct
    await new FixedRefDataSeeder(db, roles).Seed();
  }
  
  /// <summary>
  /// Do any app initialisation that requires the host builder (before the app is built)
  ///
  /// This is less commonly required
  /// </summary>
  /// <param name="b"></param>
  public static async Task Initialise(this WebApplicationBuilder b)
  {
    await Task.CompletedTask;
  }

}
