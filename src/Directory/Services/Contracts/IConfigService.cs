using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Services.Contracts
{
    public interface IConfigService
    {
        IEnumerable<Config> ListSiteConfigs(string wildcard = "");

        Task<IEnumerable<Config>> ListSiteConfigsAsync(string wildcard = "");

        Task<Config> GetSiteConfig(string key);

        Task<string> GetSiteConfigValue(string key, string defaultValue = "");

        Task<bool> GetSiteConfigStatus(string siteConfigValue);

        Task UpdateSiteConfigsAsync(IEnumerable<Config> configs);

    }
}
