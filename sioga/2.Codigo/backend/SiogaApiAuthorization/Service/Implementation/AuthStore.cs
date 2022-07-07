using Microsoft.Extensions.Caching.Memory;
using SiogaApiAuthorization.Service.Contracts;
using System;
using System.Threading.Tasks;

namespace SiogaApiAuthorization.Service.Implementation
{
    public class AuthStore : IAuthStore
    {
        private readonly IMemoryCache _memoryCache;

        public AuthStore(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public bool AddStore(string key, string value)
        {
            if (!String.IsNullOrEmpty(key))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
                _memoryCache.Set(key, value, cacheEntryOptions);
                return true;
            }

            return false;

        }

        public async Task<string> GetStore(string key)
        {
            if (!_memoryCache.TryGetValue(key, out string value))
            {
                value = null;
            }

            return await Task.FromResult<string>(value);
        }
    }
}
