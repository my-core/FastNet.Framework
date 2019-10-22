using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TCM.Rich.Caching;
using TCM.Rich.ConfigurationOptions;

namespace TCM.SuperX.DiceService.Managers
{
    public class RedisLockCacheManager : IRedisLockCacheManager
    {
        private readonly RedisOptions _redisOptions;
        private readonly RedisCache _cache;
        private readonly TimeSpan timeSpan = TimeSpan.FromSeconds(7);
        private readonly string _redisLockPrefix = "DiceRedisLock";
        public RedisLockCacheManager(
            IOptionsFactory<RedisOptions> options)
        {
            _redisOptions = options.Create(this.GetType().Name);
            _cache = new RedisCache(_redisOptions);
            _cache.Connect();
        }

        public async Task<bool> LockAsync(string key)
        {
            key = $"{_redisLockPrefix}-{key}";
            return await _cache.LockAsync(key, timeSpan);
        }

        public async Task UnLockAsync(string key)
        {
            key = $"{_redisLockPrefix}-{key}";
            await _cache.UnLockAsync(key);
        }
    }
}
