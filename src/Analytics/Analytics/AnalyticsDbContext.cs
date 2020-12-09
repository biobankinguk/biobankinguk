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
        public DbSet<Organisation> Organisations { get; set; }

        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }
    }

}
