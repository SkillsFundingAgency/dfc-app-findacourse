namespace DFC.App.FindACourse.Cache
{
    public interface ICacheService
    {
        public void AddOrUpdateCache(string cacheKey, object entry);

        public T GetFromCache<T>(string key)
            where T : class;

        public T GetOrSet<T>(string cacheKey, object entry)
            where T : class;
    }
}
