using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Directory.Services.Directory.Enums;

namespace Biobanks.Directory.Services.Directory.Contracts
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
        IEnumerable<Data.Entities.Config> ListSiteConfigs(string wildcard = "");

        /// <summary>
        /// Async version of ListSiteConfigs()
        /// </summary>
        /// <param name="wildcard"> Optional, specifies area/type of config </param>
        /// <returns>List of Site Configs</returns>
        Task<IEnumerable<Data.Entities.Config>> ListSiteConfigsAsync(string wildcard = "");

        /// <summary>
        /// Get Site Config given its key
        /// </summary>
        /// <param name="key">Config key</param>
        /// <returns>Config obj</returns>
        Task<Data.Entities.Config> GetSiteConfig(string key);

        /// <summary>
        /// Gets site Config value given its key
        /// </summary>
        /// <param name="configKey">Config key</param>
        /// <param name="defaultValue">Default value string</param>
        /// <param name="checkCacheFirst">bool to check if cache is avaible</param>
        /// <returns>Config value string</returns>
        Task<string> GetSiteConfigValue(string configKey, string defaultValue = "", bool checkCacheFirst = false);

        /// <summary>
        /// Updates specified Configs
        /// </summary>
        /// <param name="configs">List of Config objs to be updatd</param>
        /// <returns></returns>
        Task UpdateSiteConfigsAsync(IEnumerable<Data.Entities.Config> configs);

        /// <summary>
        /// Updates cached config values.
        /// </summary>
        /// <returns></returns>
        Task PopulateSiteConfigCache();

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
        Task<List<Data.Entities.Config>> ListBooleanFlags(BooleanConfigSelection selection, string wildcard = "");

        /// <summary>
        /// Updates boolean flag value
        /// </summary>
        /// <param name="key">Config key</param>
        /// <param name="value">Config bool value</param>
        /// <returns></returns>
        Task UpdateFlag(string key, bool value);


    }
}
