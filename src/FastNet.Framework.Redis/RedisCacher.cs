using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastNet.Framework.Redis
{
    /// <summary>
    /// Redis缓存器基类
    /// </summary>
    public class RedisCacher : IRedisCacher
    {
        private RedisOptions _options;
        private IDatabase _database;
        private ConnectionMultiplexer _cacheServer = null;

        public RedisCacher(RedisOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// 连接redis
        /// </summary>
        public void Connect()
        {
            var options = new ConfigurationOptions()
            {
                AllowAdmin = false,// Enables a range of commands that are considered risky
                ConnectTimeout = 5000,// Timeout (ms) for connect operations
                KeepAlive = 5,// Time (seconds) at which to send a message to help keep sockets alive
                SyncTimeout = 5000,// Time (ms) to allow for synchronous operations
                Ssl = false
            };
            //判断是否设置密码
            if (!string.IsNullOrWhiteSpace(_options.Password))
            {
                options.Password = _options.Password;
            }
            //redis结点
            _options.EndPoints.ForEach(item =>
            {
                options.EndPoints.Add(item);
            });
            //创建connection实例
            _cacheServer = ConnectionMultiplexer.Connect(options);
            //获取数据库交互实例
            _database = _cacheServer.GetDatabase(_options.DatabaseNo);
        }

        #region String Value
        /// <summary>
        /// 设置key缓存String类型数据（永不过期）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        public void Set<T>(string key, T t)
        {
            _database.StringSet(key, JsonConvert.SerializeObject(t));
        }

        /// <summary>
        /// 设置key缓存String类型数据（有期效）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="expiry">到期时间</param>
        public void Set<T>(string key, T t, TimeSpan expiry)
        {
            _database.StringSet(key, JsonConvert.SerializeObject(t), expiry);
        }

        /// <summary>
        /// 获取key缓存String类型数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            var result = JsonConvert.DeserializeObject<T>(_database.StringGet(key));
            return result;
        }

        /// <summary>
        /// 设置新数据并返回旧数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public T GetSet<T>(string key, T newValue)
        {
            var value = _database.StringGetSet(key, JsonConvert.SerializeObject(newValue));
            if (value.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            else
            {
                return default(T);
            }
        }
        #endregion

        #region Hash Value        
        /// <summary>
        /// 设置Key缓存Hash类型数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="t"></param>
        public void SetHash<T>(string key, string hashField, T t)
        {
            _database.HashSet(key, hashField, JsonConvert.SerializeObject(t));
        }

        /// <summary>
        /// 设置Key缓存Hash类型数据(批量)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="ts"></param>
        public void SetHash<T>(string key, IDictionary<string, T> ts)
        {
            var hasEntries = new HashEntry[ts.Count];
            var entries = ts.ToList();
            for (var i = 0; i < ts.Count; i++)
            {
                hasEntries[i] = new HashEntry(entries[i].Key, JsonConvert.SerializeObject(entries[i].Value));
            }
            _database.HashSet(key, hasEntries);
        }

        /// <summary>
        /// 自增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="incrementStep"></param>
        /// <returns></returns>
        public long HashIncrement(string key, string hashField, int incrementStep = 1)
        {
            return _database.HashIncrement(key, hashField, incrementStep);
        }

        /// <summary>
        ///自减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="incrementStep"></param>
        /// <returns></returns>
        public long HashDecrement(string key, string field, int incrementStep = 1)
        {
            return _database.HashDecrement(key, field, incrementStep);
        }

        /// <summary>
        /// 获取Key缓存数据(全部)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetHash<T>(string key)
        {
            var result = new List<T>();
            var hashValues = _database.HashValues(key);
            foreach (var value in hashValues)
            {
                result.Add(JsonConvert.DeserializeObject<T>(value));
            }
            return result;
        }

        /// <summary>
        /// 获取Key缓存数据(指定hashFiled)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public T GetHash<T>(string key, string hashField)
        {
            var value = _database.HashGet(key, hashField);
            if (value.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 获取Key缓存数据(指定hashFiled)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashFields"></param>
        /// <returns></returns>
        public List<T> GetHash<T>(string key, List<string> hashFields)
        {
            var result = new List<T>();
            var values = _database.HashGet(key, hashFields.Select(a => (RedisValue)a).ToArray());
            if (values != null && values.Count() > 0)
            {
                foreach (var value in values)
                {
                    if (value.HasValue)
                    {
                        result.Add(JsonConvert.DeserializeObject<T>(value));
                    }
                }
            }
            return result;
        }

        #endregion

        #region Exist/Get/Remove Key
        /// <summary>
        /// 是否存在key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _database.KeyExists(key);
        }

        /// <summary>
        /// 获取指定通配的所有key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IList<string> GetAllKeys(string pattern)
        {
            var keys = new List<string>();
            var endPoints = _cacheServer.GetEndPoints();
            foreach (var endPoint in endPoints)
            {
                var server = _cacheServer.GetServer(endPoint);
                var serverKeys = server.Keys(_options.DatabaseNo, pattern + "*");
                keys.AddRange(serverKeys.Select(k => k.ToString()));
            }
            return keys;
        }

        /// <summary>
        /// 获取指定通配的所有key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IList<string> GetAllKeysByScript(string pattern)
        {
            var keys = new List<string>();
            var redisResult = _database.ScriptEvaluateAsync(LuaScript.Prepare(@" local res = redis.call('KEYS', @keypattern) " + " return res "), new { @keypattern = pattern + "*" }).GetAwaiter().GetResult();
            if (!redisResult.IsNull)
                keys.AddRange((string[])redisResult);

            return keys;
        }

        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            _database.KeyDelete(key);
        }

        /// <summary>
        /// 删除指定通配的所有key
        /// </summary>
        /// <param name="pattern"></param>
        public void RemoveAll(string pattern)
        {
            var endPoints = _cacheServer.GetEndPoints();
            foreach (var endPoint in endPoints)
            {
                var server = _cacheServer.GetServer(endPoint);
                var serverKeys = server.Keys(_options.DatabaseNo, pattern + "*");
                _database.KeyDelete(serverKeys.ToArray());
            }
        }

        /// <summary>
        /// 删除指定通配的所有key
        /// </summary>
        /// <param name="pattern"></param>
        public void RemoveAllByScript(string pattern)
        {
            var keys = new List<string>();
            var redisResult = _database.ScriptEvaluateAsync(LuaScript.Prepare(@" local res = redis.call('KEYS', @keypattern) " + " return res "), new { @keypattern = pattern + "*" }).GetAwaiter().GetResult();
            if (!redisResult.IsNull)
                _database.KeyDelete((RedisKey[])redisResult);
        }

        #endregion

        #region Redis Lock
        /// <summary>
        /// 对key加锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <param name="flags"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> LockAsync(string key, TimeSpan expiry, CommandFlags flags = CommandFlags.None, string value = "1")
        {
            return await _database.LockTakeAsync(key, value, expiry, flags);
        }
        /// <summary>
        /// 对key释放锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task UnLockAsync(string key, CommandFlags flags = CommandFlags.None, string value = "1")
        {
            await _database.LockReleaseAsync(key, value, flags);
        }
        #endregion
    }
}
