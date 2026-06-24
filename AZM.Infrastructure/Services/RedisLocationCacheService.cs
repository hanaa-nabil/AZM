using AZM.Domain.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Services
{
    public class RedisLocationCacheService : ILocationCacheService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisLocationCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task SetLocationAsync(string sessionId, string userId, double lat, double lng)
        {
            var db = _redis.GetDatabase();
            var key = $"session:{sessionId}:loc:{userId}";
            var value = $"{lat},{lng}";
            await db.StringSetAsync(key, value, TimeSpan.FromMinutes(10));
        }

        public async Task<Dictionary<string, (double lat, double lng)>> GetAllLocationsAsync(string sessionId)
        {
            var db = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var pattern = $"session:{sessionId}:loc:*";
            var result = new Dictionary<string, (double, double)>();

            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                var value = await db.StringGetAsync(key);
                var parts = value.ToString().Split(',');
                var userId = key.ToString().Split(':').Last();
                result[userId] = (double.Parse(parts[0]), double.Parse(parts[1]));
            }

            return result;
        }
    }
}
