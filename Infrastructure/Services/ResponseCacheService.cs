using Core.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<ResponseCacheService> _logger;
        public ResponseCacheService(IConnectionMultiplexer redis,
            ILogger<ResponseCacheService> logger)
        {
            _redis = redis;
            _logger = logger;
        }
        public async Task CacheResponseAsync<T>(string cacheKey, T response, TimeSpan timeToLive)
        {
            try
            {
                if (!_redis.IsConnected) return;

                var db = _redis.GetDatabase();
                var serialisedResponse = JsonSerializer.Serialize(response);

                await db.StringSetAsync(cacheKey, serialisedResponse, timeToLive);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to write to Redis: {ex.Message}");
                // Ignores the write failure so the user still gets their data
            }

        }

        public async Task<T> GetCachedResponseAsync<T>(string cacheKey)
        {
            try
            {
                // 1. Check if Redis is actually connected before trying
                if (!_redis.IsConnected)
                    return default;

                var db = _redis.GetDatabase();
                var cachedResponse = await db.StringGetAsync(cacheKey);

                if (cachedResponse.IsNullOrEmpty)
                    return default;

                return JsonSerializer.Deserialize<T>((string)cachedResponse);
            }
            catch (Exception ex)
            {
                // 2. LOG the error, but DO NOT crash.
                _logger.LogError($"Redis is down or unreachable: {ex.Message}");

                // 3. Return null so the app thinks "Cache Miss" and goes to the DB
                return default;
            }
        }

       
    }
}
