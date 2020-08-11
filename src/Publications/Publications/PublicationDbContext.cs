using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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

    // Ideally this shouldnt be in the class library
    public class PublicationDbContextFactory : IDesignTimeDbContextFactory<PublicationDbContext>
    {
        public PublicationDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<PublicationDbContext>();
            options.UseSqlServer(Environment.GetEnvironmentVariable("sqldb_connection"));

            return new PublicationDbContext(options.Options);
        }
    }
}
