using Biobanks.Directory.Data;
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
    /// <inheritdoc cref="IConfigService" />
    public class ConfigService : IConfigService
    {
        private readonly BiobanksDbContext _db;
        public ConfigService(BiobanksDbContext db)
        {
            _db = db;
        }
        public enum BooleanConfigSelection
        {
            FeatureFlagsOnly,
            ExcludeFeatureFlags,
            AllBooleanConfigs
        }

        /// <inheritdoc />
        public IEnumerable<Config> ListSiteConfigs(string wildcard = "")
            => _db.Configs.Where(x => x.Key.Contains(wildcard)).ToList();

        /// <inheritdoc />
        public async Task<IEnumerable<Config>> ListSiteConfigsAsync(string wildcard = "")
            => await _db.Configs.Where(x => x.Key.Contains(wildcard)).ToListAsync();

        /// <inheritdoc />
        public async Task<Config> GetSiteConfig(string key)
             => (await ListSiteConfigsAsync(key)).FirstOrDefault();

        /// <inheritdoc />
        public async Task<string> GetSiteConfigValue(string key, string defaultValue = "")
            => (await GetSiteConfig(key))?.Value ?? defaultValue;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<bool?> GetFlagConfigValue(string key)
            => (ConvertStringToBool((await GetSiteConfig(key)).Value));

        /// <inheritdoc />
        public async Task<List<Config>> ListBooleanFlags(BooleanConfigSelection selection, string wildcard = "")
        {
            var configs = await _db.Configs.Where(x => x.Key.Contains(wildcard)).ToListAsync();
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
            _db.Configs.AddOrUpdate(oldConfig);

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
