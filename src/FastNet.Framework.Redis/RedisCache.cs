
using Newtonsoft.Json;
using NLog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastNet.Framework.Redis
{
    /// <summary>
    /// Redis操作基类
    /// </summary>
    public class RedisCache 
    {
        Logger _logger = LogManager.GetCurrentClassLogger();
        private RedisOptions _options;
        private IDatabase _database;
        private ConnectionMultiplexer _cacheServer = null;

        public RedisCache(RedisOptions options)
        {
            _options = options;
        }
        /// <summary>
        /// 建立redis连接
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
            options.EndPoints.Add(_options.EndPoint);
            _cacheServer = ConnectionMultiplexer.Connect(options);
            _database = _cacheServer.GetDatabase(_options.DatabaseNo);

            _logger.Info($"RedisClient Connected, EndPoints[{_options.EndPoint}], DatabaseNo[{_options.DatabaseNo}]");
        }

        /// <summary>
        /// 设置key缓存数据（永不过期）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        public void Set<T>(string key, T t)
        {
            _database.StringSet(key, JsonConvert.SerializeObject(t));
        }
        
        /// <summary>
        /// 
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
        /// 获取key缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(_database.StringGet(key));
                return result;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Failed to get value of key[{key}]");
                return default(T);
            }
        }
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
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            _database.KeyDelete(key);
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
        /// <summary>
        /// 对key加锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <param name="flags"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> LockAsync(string key,TimeSpan expiry,CommandFlags flags=CommandFlags.None,string value="1")
        {
            return await _database.LockTakeAsync(key, value, expiry,flags);
        }
        /// <summary>
        /// 对key释放锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task UnLockAsync(string key,CommandFlags flags=CommandFlags.None,string value="1")
        {
            await _database.LockReleaseAsync(key, value, flags);
        }
        /// <summary>
        /// 开始连接redis
        /// </summary>
        public void Start()
        {
            Connect();
        }
    }
}
