using Core.Entities;
using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class BasketRepository : IBasketRepository
    {
        //private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<ResponseCacheService> _logger;
        private readonly IMemoryCache _memoryCache; // Local RAM Fallback

        public BasketRepository(IConnectionMultiplexer redis,
            IMemoryCache memoryCache,
            ILogger<ResponseCacheService> logger)
        {
            _redis = redis;
            //_database = redis.GetDatabase();
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            try
            {
                if (_redis.IsConnected)
                    return await _redis.GetDatabase().KeyDeleteAsync(basketId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis unreachable during Delete.");
            }
            _memoryCache.Remove(basketId);
            return true;
        }

        public async Task<CustomerBasket> GetBasketAsync(string basketId)
        {
            try
            {
                if (_redis.IsConnected)
                {
                    var data = await _redis.GetDatabase().StringGetAsync(basketId);
                    if (!data.IsNullOrEmpty)
                        return JsonSerializer.Deserialize<CustomerBasket>((string)data!);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Redis unreachable. Falling back to Memory Cache for Get.");
            }
            return _memoryCache.Get<CustomerBasket>(basketId);

        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket)
        {
            var expiry = TimeSpan.FromDays(15);
            try
            {
                if (_redis.IsConnected)
                {
                    var json = JsonSerializer.Serialize(customerBasket);
                    var created = await _redis.GetDatabase().StringSetAsync(customerBasket.Id, json, expiry);
                    if (created) return await GetBasketAsync(customerBasket.Id);
                }

            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex, "Redis unreachable. Falling back to Memory Cache for Update.");
               
            }

            // Fallback to Local RAM
            _memoryCache.Set(customerBasket.Id, customerBasket, expiry);
            return customerBasket;
        }
    }
}
