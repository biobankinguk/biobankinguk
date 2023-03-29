using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Biobanks.Submissions.Api.Startup.Web;

public static class WebEntrypoint
{
  public static async Task Run(string[] args)
  {
    var b = WebApplication.CreateBuilder(args);

    // Configure DI Services
    b.ConfigureServices();

    // Initialisation before building the app
    await b.Initialise();

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
