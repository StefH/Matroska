using System;
using System.Runtime.Caching;

namespace Matroska.Extensions
{
    public static class ObjectCacheExtensions
    {
        private static CacheItemPolicy CacheItemPolicy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddYears(99)
        };

        public static T AddOrGetExisting<T>(this ObjectCache cache, string key, Func<T> valueFactory)
        {
            var newValue = new Lazy<T>(valueFactory);
            var oldValue = cache.AddOrGetExisting(key, newValue, CacheItemPolicy) as Lazy<T>;

            try
            {
                return (oldValue ?? newValue).Value;
            }
            catch
            {
                cache.Remove(key);
                throw;
            }
        }
    }
}