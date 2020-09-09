using Microsoft.EntityFrameworkCore;
using Analytics.Data.Entities;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Analytics.Data
{
    public class AnalyticsDbContext : DbContext
    {
        public DbSet<OrganisationAnalytic> OrganisationAnalytics { get; set; }
        public DbSet<DirectoryAnalyticMetric> DirectoryAnalyticMetrics { get; set; }
        public DbSet<DirectoryAnalyticEvent> DirectoryAnalyticEvents { get; set; }

        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

        /*  Required for DesignTime creation of the context. Used for running EF migrations in CLI and devops pipelines. */
        public class AnalyticsDbContextFactory : IDesignTimeDbContextFactory<AnalyticsDbContext>
        {
            public AnalyticsDbContext CreateDbContext(string[] args)
            {
                var options = new DbContextOptionsBuilder<AnalyticsDbContext>();

                // Connection string passed from environment variable as EF doesn't (yet) support supplying it as a CLI parameter
                options.UseSqlServer(Environment.GetEnvironmentVariable("analyticsdb_connection"), options => options.EnableRetryOnFailure());
                
                return new AnalyticsDbContext(options.Options);
            }
        }
    }

}
