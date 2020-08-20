using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Publications.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Publications
{
    public class PublicationDbContext : DbContext
    {
        public PublicationDbContext() : base() { }

        public PublicationDbContext(DbContextOptions<PublicationDbContext> options) : base(options) { }

        public DbSet<Publication> Publications { get; set; }
    }

    /*  Required for DesignTime creation of the context. Used for running EF migrations in CLI and devops pipelines. */
    public class PublicationDbContextFactory : IDesignTimeDbContextFactory<PublicationDbContext>
    {
        public PublicationDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<PublicationDbContext>();

            // Connection string passed from environment variable as EF doesn't (yet) support supplying it as a CLI parameter
            options.UseSqlServer(Environment.GetEnvironmentVariable("sqldb_connection"));

            return new PublicationDbContext(options.Options);
        }
    }
}
