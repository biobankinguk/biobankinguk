using Microsoft.EntityFrameworkCore;
using Analytics.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Analytics
{
    public class AnalyticsDbContext : DbContext
    {
        public DbSet<OrganisationAnalytic> OrganisationAnalytics { get; set; }
        public DbSet<DirectoryAnalyticMetric> DirectoryAnalyticMetrics { get; set; }
        public DbSet<DirectoryAnalyticEvent> DirectoryAnalyticEvents { get; set; }
        public DbSet<Organisation> Organisations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Data Source=blogging.db");
    }

}
