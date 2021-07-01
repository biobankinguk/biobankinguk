﻿using Biobanks.Directory.Data;
using Biobanks.Entities.Data;
using Biobanks.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Services
{
    public class ConfigService : IConfigService
    {
        private readonly BiobanksDbContext _db;

        public ConfigService(BiobanksDbContext db)
        {
            _db = db;
        }

        // ReadService Methods
        public IEnumerable<Config> ListSiteConfigs(string wildcard = "")
            => _db.Configs.Where(x => x.Key.Contains(wildcard)).ToList();

        public async Task<IEnumerable<Config>> ListSiteConfigsAsync(string wildcard = "")
            => await _db.Configs.Where(x => x.Key.Contains(wildcard)).ToListAsync();

        public async Task<Config> GetSiteConfig(string key)
             => (await ListSiteConfigsAsync(key)).FirstOrDefault();

        public async Task<string> GetSiteConfigValue(string key, string defaultValue = "")
            => (await GetSiteConfig(key))?.Value ?? defaultValue;

        public async Task<bool> GetSiteConfigStatus(string siteConfigValue)
        {
            return await _db.Configs.Where(x => x.Key == siteConfigValue && x.Value == "true").AnyAsync();
        }

        //WriteService Methods
        public async Task UpdateSiteConfigsAsync(IEnumerable<Config> configs)
        {
            foreach (var config in configs)
            {
                var oldConfig = await GetSiteConfig(config.Key);
                oldConfig.Value = config.Value;

                _db.Configs.AddOrUpdate(oldConfig);

            }
            await _db.SaveChangesAsync();
        }

        // Boolean typesafe methods

        // Getter - Returns boolean value of flag, null if not flag
        public async Task<bool?> GetFlagConfigValue(string key)
            => (ConvertStringToBool((await GetSiteConfig(key)).Value));

        public async Task<IEnumerable<Config>> ListFeatureFlags(string wildcard = "")
            => await _db.Configs.Where(x => x.Key.Contains(wildcard)).Where(x => x.IsFeatureFlag == true).ToListAsync();


        // Setter - Updates given flag with passed boolean value
        public async Task UpdateFlag(string key, bool value)
        {
            var oldConfig = await GetSiteConfig(key);
            oldConfig.Value = value.ToString();
            _db.Configs.AddOrUpdate(oldConfig);

            await _db.SaveChangesAsync();

        }

        /// <summary>
        /// Returns value of boolean flag
        /// If not a bool will return null
        /// </summary>
        /// <param name="boolStr"></param>
        /// <returns></returns>
        private bool? ConvertStringToBool(string boolStr)
        {
            if (Boolean.TryParse(boolStr, out var converted))
            {
                return converted;
            }
            // Return null if not a flag/bool
            return null;
        }


    }
}
