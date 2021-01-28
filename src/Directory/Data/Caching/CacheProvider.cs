using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Biobanks.Directory.Data.Caching
{
    public class CacheProvider : ICacheProvider
    {
        private readonly MemoryCache _cache;

        public CacheProvider()
        {
            _cache = MemoryCache.Default;
        }

        public void Store<T>(string cacheKey, object objectToCache, TimeSpan? timeUntilExpiry = null)
        {
            _cache.Set(cacheKey, objectToCache, timeUntilExpiry != null ? 
                DateTime.Now.Add(timeUntilExpiry.Value) : 
                DateTime.Now.AddHours(24));
        }

        public T Retrieve<T>(string cacheKey)
        {
            var val = _cache.Get(cacheKey);
            
            if(val == null) throw new KeyNotFoundException(
                $"{cacheKey} requested but not found.");

            return (T)val;
        }
    }
}
