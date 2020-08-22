using Microsoft.EntityFrameworkCore;
using Analytics.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.EntityFrameworkCore.Design;

namespace Analytics
{
    public class AnalyticsDbContext : DbContext
    {
        public DbSet<OrganisationAnalytic> OrganisationAnalytics { get; set; }
        public DbSet<DirectoryAnalyticMetric> DirectoryAnalyticMetrics { get; set; }
        public DbSet<DirectoryAnalyticEvent> DirectoryAnalyticEvents { get; set; }
        public DbSet<Organisation> Organisations { get; set; }

        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

        /*  Required for DesignTime creation of the context. Used for running EF migrations in CLI and devops pipelines. */
        public class AnalyticsDbContextFactory : IDesignTimeDbContextFactory<AnalyticsDbContext>
        {
            public AnalyticsDbContext CreateDbContext(string[] args)
            {
                var options = new DbContextOptionsBuilder<AnalyticsDbContext>();

                // Connection string passed from environment variable as EF doesn't (yet) support supplying it as a CLI parameter
                // Create ENV in CLI as such $env:analyticsdb_connection="Server=tcp:SqlServerdetails.."
                //options.UseSqlServer(Environment.GetEnvironmentVariable("analyticsdb_connection"));
                options.UseSqlServer("Server=tcp:biobank.database.windows.net,1433;Initial Catalog=biobank;Persist Security Info=False;User ID=biobank;Password=$analytics123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

                return new AnalyticsDbContext(options.Options);
            }
        }
    }

}
