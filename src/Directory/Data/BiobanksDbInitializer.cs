using Biobanks.Directory.Data.Constants;
using Biobanks.Directory.Data.Transforms.Url;
using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Biobanks.Directory.Data
{
    public class BiobanksDbInitializer : IDatabaseInitializer<BiobanksDbContext>
    {
        public void InitializeDatabase(BiobanksDbContext context)
        {
            SeedDirectoryConfigs(context);
            SeedOrganisationTypes(context);
            SeedContentPages(context);
        }

        private void SeedDirectoryConfigs(BiobanksDbContext context)
        {
            var set = context.Set<Config>();

            // Seed Missing Values
            Configs.Configs
                .DefaultConfigs
                .Where(x => !set.Any(y => y.Key == x.Key))
                .ToList()
                .ForEach(x => set.Add(x));

            context.SaveChanges();
        }

        private void SeedOrganisationTypes(BiobanksDbContext context)
        {
            var set = context.Set<OrganisationType>();

            var organisationTypes = new List<OrganisationType>
            {
                new OrganisationType
                {
                    OrganisationTypeId = 1,
                    Description = "Biobank",
                    SortOrder = 1
                }
            };

            // Seed Missing Values
            organisationTypes
                 .Where(x => !set.Any(y => y.Description == x.Description))
                 .ToList()
                 .ForEach(x => set.Add(x));

            context.SaveChanges();
        }
    
        private void SeedContentPages(BiobanksDbContext context)
        {
            var pages = new List<ContentPage>()
            {
                new ContentPage
                {
                    Title = "About",
                    Body = "",
                    RouteSlug = "About",
                    IsEnabled = true,
                    LastUpdated = DateTime.UtcNow
                },
                new ContentPage
                {
                    Title = "Privacy Policy",
                    Body = "",
                    RouteSlug = "Privacy",
                    IsEnabled = true,
                    LastUpdated = DateTime.UtcNow
                }
            };

            var NTDConfig = context.Configs.Where(x => x.Key.Contains(ConfigKey.CollectionsNotes)).FirstOrDefault();

            if (NTDConfig.Value != "false")
            {
                pages.Add(new ContentPage
                {
                    Title = "Neglected Tropical Disease Associated Data Types",
                    Body = "",
                    RouteSlug = "NTD",
                    IsEnabled = true,
                    LastUpdated = DateTime.UtcNow
                });
            }

            foreach (var page in pages)
            {
                if (!context.ContentPages.Any(x => x.RouteSlug == page.RouteSlug))
                {
                    context.ContentPages.Add(page);
                }
            }

            context.SaveChanges();
        }
    }
}
