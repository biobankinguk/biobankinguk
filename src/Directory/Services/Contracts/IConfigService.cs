using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biobanks.Services.ConfigService;

namespace Biobanks.Services.Contracts
{
    /// <summary>
    /// Domain specific service
    /// Config Services moved from read/write services
    /// Specific methods dealing with boolean flags only
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// Lists all site config options
        /// </summary>
        /// <param name="wildcard"> Optional, specifies area/type of config </param>
        /// <returns>List of Site Configs</returns>
        IEnumerable<Config> ListSiteConfigs(string wildcard = "");

        /// <summary>
        /// Async version of ListSiteConfigs()
        /// </summary>
        /// <param name="wildcard"> Optional, specifies area/type of config </param>
        /// <returns>List of Site Configs</returns>
        Task<IEnumerable<Config>> ListSiteConfigsAsync(string wildcard = "");

        /// <summary>
        /// Get Site Config given its key
        /// </summary>
        /// <param name="key">Config key</param>
        /// <returns>Config obj</returns>
        Task<Config> GetSiteConfig(string key);

        /// <summary>
        /// Gets site Config value given its key
        /// </summary>
        /// <param name="key">Config key</param>
        /// <param name="defaultValue">Default value string</param>
        /// <returns>Config value string</returns>
        Task<string> GetSiteConfigValue(string key, string defaultValue = "");

        /// <summary>
        /// Updates specified Configs
        /// </summary>
        /// <param name="configs">List of Config objs to be updatd</param>
        /// <returns></returns>
        Task UpdateSiteConfigsAsync(IEnumerable<Config> configs);

        /// <summary>
        /// Gets Flag Config Value
        /// </summary>
        /// <param name="key">Config key</param>
        /// <returns>Bool? - null if given key does not correspond to a boolean flag</returns>
        Task<bool?> GetFlagConfigValue(string key);

        /// <summary>
        /// Lists Boolean configs with option to include/exclude feature flags
        /// </summary>
        /// <param name="selection">
        /// Three different options:
        /// Feature Flags Only - Boolean flags with IsFeatureFlag == true
        /// ExcludeFeatureFlags - 
        /// AllBooleanConfigs - 
        /// </param>
        /// <param name="wildcard">Optional, specifies area/type of config</param>
        /// <returns>List of Config objs</returns>
        Task<List<Config>> ListBooleanFlags(BooleanConfigSelection selection, string wildcard = "");

        /// <summary>
        /// Updates boolean flag value
        /// </summary>
        /// <param name="key">Config key</param>
        /// <param name="value">Config bool value</param>
        /// <returns></returns>
        Task UpdateFlag(string key, bool value);


    }
}
