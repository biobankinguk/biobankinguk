using Biobanks.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Biobanks.Directory.Startup.EfCoreMigrations;

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
      .ConfigureServices((b, s) =>
      {
        var connectionString =
          b.Configuration.GetConnectionString("Default");

        s.AddDbContext<ApplicationDbContext>(
            o =>
            {
              // migration bundles don't like null connection strings (yet)
              // https://github.com/dotnet/efcore/issues/26869
              // so if no connection string is set we register without one for now.
              // if running migrations, `--connection` should be set on the command line
              // in real environments, connection string should be set via config
              // all other cases will error when db access is attempted.
              if (string.IsNullOrWhiteSpace(connectionString))
                o.UseNpgsql();
              else
                o.UseNpgsql(connectionString,
                  pgo => pgo.EnableRetryOnFailure());
            });
      })
      .Build();
  }
}
