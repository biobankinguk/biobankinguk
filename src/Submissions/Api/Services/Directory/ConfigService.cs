﻿using Biobanks.Data;
using Biobanks.Submissions.Api.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
    /// <inheritdoc cref="IConfigService" />
    public class ConfigService : IConfigService
    {
        private readonly BiobanksDbContext _db;

        private readonly IMemoryCache _memoryCache;
        public ConfigService(BiobanksDbContext db, IMemoryCache memoryCache)
        {
            _db = db;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc />
        public IEnumerable<Entities.Data.Config> ListSiteConfigs(string wildcard = "")
            => _db.Configs.Where(x => x.Key.Contains(wildcard)).ToList();

        /// <inheritdoc />
        public async Task<IEnumerable<Entities.Data.Config>> ListSiteConfigsAsync(string wildcard = "")
            => await _db.Configs.Where(x => x.Key.Contains(wildcard)).ToListAsync();

        /// <inheritdoc />
        public async Task<Entities.Data.Config> GetSiteConfig(string key)
            => (await ListSiteConfigsAsync(key)).FirstOrDefault();

        /// <inheritdoc />
        public async Task<string> GetSiteConfigValue(string key, bool checkCacheFirst, string defaultValue = "")
        {
            if (checkCacheFirst)
            {
                return (string)_memoryCache.Get($"SiteConfig/{key}");
            }
            else return (await GetSiteConfig(key))?.Value ?? defaultValue;
        }

        /// <inheritdoc />
        public async Task UpdateSiteConfigsAsync(IEnumerable<Entities.Data.Config> configs)
        {
            foreach (var config in configs)
            {
                var oldConfig = await GetSiteConfig(config.Key);
                oldConfig.Value = config.Value;

                _db.Configs.Update(oldConfig);

            }
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<bool?> GetFlagConfigValue(string key)
            => (ConvertStringToBool((await GetSiteConfig(key)).Value));

        /// <inheritdoc />
        public async Task<List<Entities.Data.Config>> ListBooleanFlags(BooleanConfigSelection selection, string wildcard = "")
        {
            var configs = await _db.Configs.Where(x => x.Key.Contains(wildcard)).Where(x => x.Name != null).ToListAsync();
            var boolConfigs = configs.Where(x => ConvertStringToBool(x.Value) != null).ToList();

            if (selection == BooleanConfigSelection.AllBooleanConfigs)
            {
                return boolConfigs;
            }
            else if (selection == BooleanConfigSelection.FeatureFlagsOnly)
            {
                return boolConfigs.Where(x => x.IsFeatureFlag == true).ToList();
            }
            else
            {
                return boolConfigs.Where(x => x.IsFeatureFlag == false || x.IsFeatureFlag == null).ToList();
            }

        }

        /// <inheritdoc />
        public async Task UpdateFlag(string key, bool value)
        {
            var oldConfig = await GetSiteConfig(key);
            oldConfig.Value = value.ToString();
            _db.Configs.Update(oldConfig);

            await _db.SaveChangesAsync();
        }
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
