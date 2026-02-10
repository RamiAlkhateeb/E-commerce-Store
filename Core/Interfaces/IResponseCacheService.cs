using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IResponseCacheService
    {
        Task<T?> GetCachedResponseAsync<T>(string cacheKey);
        Task CacheResponseAsync<T>(string cacheKey, T response, TimeSpan timeToLive);
    }
}
