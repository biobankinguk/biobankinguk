using Biobanks.Directory.Data;
using Biobanks.Entities.Data;
using Biobanks.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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


    }
}
