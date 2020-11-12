using DFC.App.FindACourse.Data.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DFC.App.FindACourse.Cache
{
    public class CacheService : ICacheService
    {
        private readonly MemoryCache cache;

        public CacheService(MemoryCache cache)
        {
            this.cache = cache;
        }

        public void AddOrUpdateCache(string cacheKey, object entry)
        {
            this.cache.TryGetValue(cacheKey, out object key);
            if (key == null)
            {
                this.cache.Set(cacheKey, entry);
            }
        }

        public T GetOrSet<T>(string cacheKey, object entry)
            where T : class
        {
            this.cache.TryGetValue(cacheKey, out object key);
            if (key == null)
            {
                this.cache.Set(cacheKey, entry);
            }

            return this.cache.Get<T>(cacheKey) as T;
        }

        public T GetFromCache<T>(string key)
            where T : class
        {
            return this.cache.Get<T>(key);
        }
    }
}
