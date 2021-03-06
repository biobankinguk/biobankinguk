﻿using Biobanks.Directory.Data.Transforms.Url;
using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace Biobanks.Directory.Data
{
    public class BiobanksDbInitializer : IDatabaseInitializer<BiobanksDbContext>
    {
        public void InitializeDatabase(BiobanksDbContext context)
        {
            SeedDirectoryConfigs(context);
            SeedOrganisationTypes(context);
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
    }
}
