using System;

namespace Directory.Data.Caching
{
    public interface ICacheProvider
    {
        /// <summary>
        /// Stores data in the application cache.
        /// </summary>
        /// <typeparam name="T">The type of item to store in the cache.</typeparam>
        /// <param name="cacheKey">The key the identifies the item in the cache.</param>
        /// <param name="objectToCache">The object to be cached.</param>
        /// <param name="timeUntilExpiry">Optional (default is 24 hours). Time after which the cached value will expire.</param>
        void Store<T>(string cacheKey, object objectToCache, TimeSpan? timeUntilExpiry = null);

        /// <summary>
        /// Retrieves data from the application cache.
        /// </summary>
        /// <typeparam name="T">The type of item to store in the cache.</typeparam>
        /// <param name="cacheKey">The key the identifies the item in the cache.</param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the specified key is not in the cache</exception>
        T Retrieve<T>(string cacheKey);
    }
}
