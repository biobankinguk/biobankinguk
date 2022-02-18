using Microsoft.EntityFrameworkCore;

namespace Omop
{
    public class OmopDbContext : DbContext
    {
        public DbSet<ConditionOccurence> ConditionOccurences { get; set; }

        public OmopDbContext() : base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            Debug.WriteLine("OnConfiguring was called!");

            // Generally use lazy loading.
            optionsBuilder.UseLazyLoadingProxies();
        }


        protected override void OnModelCreating(ModelBuilder model)
        {
            // Indices (for unique constraints)
            model.Entity<ConditionOccurence>()
                .HasIndex(x => new
                {
                    x.ConditionOccurenceId

                }).IsUnique();
        }

        public OmopDbContext(DbContextOptions options) : base(options)
        {


        }

        public class OmopDbContextFactory : IDesignTimeDbContextFactory<OmopDbContext>
        {
            /*  Required for DesignTime creation of the context. EF operations can be done from
             *  either Package Manager or via Dotnet EF Tools.
             *  
             *  For either option, the connection string of the database must be passed via
             *  CLI arguments.
             *  
             *  Package Manager - https://docs.microsoft.com/en-us/ef/core/cli/powershell
             *  >> <Migrations Command> -Args "<Connection-String>"
             *  
             *  dotnet CLI - https://docs.microsoft.com/en-us/ef/core/cli/dotnet
             *  >> dotnet ef <CLI Command> -- "<Connection-String>"
             */

            public OmopDbContext CreateDbContext(string[] args)
                => new(
                    new DbContextOptionsBuilder()

                        .UseNsqlServer(args[0], opts => opts.EnableRetryOnFailure())
                        .Options
                    );
        }
    }
}
