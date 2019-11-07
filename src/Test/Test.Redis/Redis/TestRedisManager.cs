using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FastNet.Framework.Redis;

namespace Test.Redis
{
    public class TestRedisManager: ITestRedisManager
    {
        private readonly RedisOptions _redisOptions;
        private readonly IRedisCacher _redisCacher;

        private readonly string _stringKeyPrefix = "test_";
        private readonly string _redisLockPrefix = "lock_";

        private readonly string _userKey = "user";
        public TestRedisManager(IOptionsFactory<RedisOptions> options)
        {
            _redisOptions = options.Create(this.GetType().Name);
            _redisCacher = new RedisCacher(_redisOptions);
            _redisCacher.Connect();
        }

        public void SetStringKey(string key,string value)
        {
            key = $"{_stringKeyPrefix}{key}";
            _redisCacher.Set(key, value);
        }
        public string GetStringKey(string key)
        {
            key = $"{_stringKeyPrefix}{key}";
            return _redisCacher.Get<string>(key);
        }
        public void RemoveStringKey(string key)
        {
            key = $"{_stringKeyPrefix}{key}";
            _redisCacher.Remove(key);
        }

        public void SetUserInfo(string key,string hashField, UserInfo value)
        {
            key = $"{_stringKeyPrefix}{key}";
            _redisCacher.SetHash<UserInfo>(key, hashField, value);
        }
        public UserInfo GetUserInfo(string key,int userID)
        {
            return _redisCacher.GetHash<UserInfo>(key, userID.ToString());
        }

        public void SetUserList(string key, List<UserInfo> value)
        {
            key = $"{_stringKeyPrefix}{key}";
            value.ForEach(item =>
            {
                _redisCacher.SetHash<UserInfo>(key, item.UserID.ToString(), item);
            });
        }

        public async Task<bool> LockAsync(string key)
        {
            key = $"{_redisLockPrefix}{key}";
            return await _redisCacher.LockAsync(key, TimeSpan.FromSeconds(30));
        }

        public async Task UnLockAsync(string key)
        {
            key = $"{_redisLockPrefix}{key}";
            await _redisCacher.UnLockAsync(key);
        }
    }
}
