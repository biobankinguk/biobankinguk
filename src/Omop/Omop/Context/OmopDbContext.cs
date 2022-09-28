using Biobanks.Omop.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Omop.Context
{
    public class OmopDbContext : DbContext
    {
        public DbSet<ConditionOccurence> ConditionOccurences { get; set; }


        public OmopDbContext(DbContextOptions<OmopDbContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }


        protected override void OnModelCreating(ModelBuilder model)
        {
            // Indices (for unique constraints)
            model.Entity<ConditionOccurence>()
                .HasIndex(x => new
                {
                    x.ConditionOccurenceId

                });
        }
    }
}
