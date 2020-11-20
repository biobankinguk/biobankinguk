using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;
using Directory.Identity.Constants;
using Directory.Identity.Data.Entities;

namespace Directory.Identity.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<UserStoreDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            // Needed any time your migrations aren't in a
            // "Migrations" directory at the top level of the project
            MigrationsDirectory = @"Data\Migrations";

            // Leaving this the same as the original Migrations namespace forever
            // Makes changing the namespace easier without any faff in existing DBs
            ContextKey = "Biobanks.Data.IdentityMigrations.Configuration";
        }

        protected override void Seed(UserStoreDbContext context)
        {
            // Seed ROLES!

            // These are still seeded here as the application depends on them anyway,
            // so they already intrinsically code-driven
            // adding a new role to the DB outside of a code change would have no effect.
            foreach (Role role in Enum.GetValues(typeof(Role)))
            {
                context.Roles.AddOrUpdate(x => x.Name, new ApplicationRole(role.ToString()));
            }

            // We no longer seed any users as they should be per environment
            // Use the `add-user.sql` script in the repo.
        }
    }
}
