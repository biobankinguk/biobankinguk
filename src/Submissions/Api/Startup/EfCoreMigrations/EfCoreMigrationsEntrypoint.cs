using Biobanks.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Biobanks.Submissions.Api.Startup.EfCoreMigrations;

public static class EfCoreMigrations
{
  // EF Core needs an unambiguous unconditional DbContext configuration
  // to use for Migrations at design time
  // So we build a lightweight Generic Host here
  // AddDbContext
  // and then throw it away.
  // Then we run this unconditionally in Program.cs before any other entrypoints
  // so that the EF Core tooling always works.
  public static void BootstrapDbContext(string[] args)
  {
    using var _ = Host.CreateDefaultBuilder(args)
      .ConfigureServices((b, s) => s
        .AddDbContext<ApplicationDbContext>(
          o => o.UseNpgsql(
            b.Configuration.GetConnectionString("Default"),
            pgo => pgo.EnableRetryOnFailure())))
      .Build();
  }
}
