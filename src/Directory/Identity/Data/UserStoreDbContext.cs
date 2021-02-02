using System.Data.Entity;
using Biobanks.Identity.Data.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Biobanks.Identity.Data
{
    public class UserStoreDbContext : IdentityDbContext<ApplicationUser, ApplicationRole,
            string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public virtual DbSet<ApplicationGroup> ApplicationGroups { get; set; }
        /// <summary>
        /// Really we shouldn't need this, but EF's not picking up changes in navigation collections for some reason.
        /// </summary>
        public virtual DbSet<ApplicationUserGroup> ApplicationUserGroups { get; set; }
        /// <summary>
        /// Really we shouldn't need this, but EF's not picking up changes in navigation collections for some reason.
        /// </summary>
        public virtual DbSet<ApplicationGroupRole> ApplicationGroupRoles { get; set; }

        public UserStoreDbContext() : base("Biobanks") { }
        public UserStoreDbContext(string connectionString) : base(connectionString) { }

        // Override OnModelsCreating:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationUserGroup>(g => g.ApplicationUsers)
                .WithRequired().HasForeignKey<string>(ag => ag.ApplicationGroupId);
            modelBuilder.Entity<ApplicationUserGroup>()
                .HasKey(r =>
                    new
                    {
                        ApplicationUserId = r.ApplicationUserId,
                        ApplicationGroupId = r.ApplicationGroupId
                    }).ToTable("ApplicationUserGroups");

            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationGroupRole>(g => g.ApplicationRoles)
                .WithRequired().HasForeignKey<string>(ap => ap.ApplicationGroupId);
            modelBuilder.Entity<ApplicationGroupRole>().HasKey(gr =>
                new
                {
                    ApplicationRoleId = gr.ApplicationRoleId,
                    ApplicationGroupId = gr.ApplicationGroupId
                }).ToTable("ApplicationGroupRoles");
        }
    }
}
