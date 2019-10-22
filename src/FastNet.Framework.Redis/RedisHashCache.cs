
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
    public class RedisHashCache
    {
        Logger _log = LogManager.GetCurrentClassLogger();
        private RedisOptions _options;
        private IDatabase _database;
        private ConnectionMultiplexer _cacheServer = null;

        public RedisHashCache(RedisOptions options)
        {
            _options = options;
        }

        public int DatabaseNo { get { return _options.DatabaseNo; } }

        public void Connect()
        {
            string cacheServerPassword = null;
            if (!string.IsNullOrWhiteSpace(_options.Password))
            {
                cacheServerPassword = Encoding.Unicode.GetString(Convert.FromBase64String(_options.Password));
            }

            var options = new StackExchange.Redis.ConfigurationOptions();
            options.AllowAdmin = false; // Enables a range of commands that are considered risky
            options.ConnectTimeout = 5000; // Timeout (ms) for connect operations
            options.KeepAlive = 5; // Time (seconds) at which to send a message to help keep sockets alive
            options.SyncTimeout = 5000; // Time (ms) to allow for synchronous operations
            options.Ssl = false;

            if (!string.IsNullOrWhiteSpace(cacheServerPassword))
            {
                options.Password = cacheServerPassword;
            }
            options.EndPoints.Add(_options.EndPoint);
            _cacheServer = ConnectionMultiplexer.Connect(options);
            _database = _cacheServer.GetDatabase(_options.DatabaseNo);

            _log.Warn($"RedisClient Connected, EndPoints[{_options.EndPoint}], DatabaseNo[{_options.DatabaseNo}]");
        }
        /// <summary>
        /// Redis Command: HSET; using a specified attribute value of T as the filed name.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="hashFieldFunc">Get field name from T</param>
        public void SetField<T>(string key, T t, Func<T, string> hashFieldFunc)
        {
            var hashFielddKey = hashFieldFunc(t);
            _database.HashSet(key, hashFielddKey, JsonConvert.SerializeObject(t));
        }

        public void SetField<T>(string key, string field, T t)
        {
            _database.HashSet(key, field, JsonConvert.SerializeObject(t));
        }
        public long FieldIncrement(string key, string field, int incrementStep = 1)
        {
            return _database.HashIncrement(key, field, incrementStep);
        }
        public long FieldDecrement(string key, string field, int incrementStep = 1)
        {
            return _database.HashDecrement(key, field, incrementStep);
        }

        /// <summary>
        /// Redis Command: HMSET;
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ts"></param>
        /// <param name="hashFieldFunc"></param>
        public void SetMultiFileds<T>(string key, List<T> ts, Func<T, string> hashFieldFunc)
        {
            var hasEntries = new HashEntry[ts.Count];
            for (var i = 0; i < ts.Count; i++)
            {
                hasEntries[i] = new HashEntry(hashFieldFunc(ts[i]), JsonConvert.SerializeObject(ts[i]));
            }
            _database.HashSet(key, hasEntries);
        }

        public void SetMultiFileds<T>(string key, IDictionary<string, T> ts)
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
        /// Redis Command: HVALS;
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetAllValues<T>(string key)
        {
            var result = new List<T>();
            try
            {
                var hashValues = _database.HashValues(key);
                foreach (var value in hashValues)
                {
                    result.Add(JsonConvert.DeserializeObject<T>(value));
                }
                return result;
            }
            catch (Exception ex)
            {
                _log.Error($"Error occured when get all values for key[{key}], ex[{ex.ToString()}]");
                return result;
            }
        }

        /// <summary>
        /// Redis Command: HashGet;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashFields"></param>
        /// <returns></returns>
        public List<T> GetValues<T>(string key, List<string> hashFields)
        {
            var result = new List<T>();
            try
            {
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
            catch (Exception ex)
            {
                _log.Error($"Error occured when get values for key[{key}], ex[{ex.ToString()}]");
                return result;
            }
        }
        /// <summary>
        /// Redis Command: HGET
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public T GetField<T>(string key, string hashField)
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

        public void Set<T>(string key, T t)
        {
            _database.StringSet(key, JsonConvert.SerializeObject(t));
        }
        public void Set<T>(string key, T t, int expiryDays)
        {
            _database.StringSet(key, JsonConvert.SerializeObject(t), TimeSpan.FromDays(expiryDays));
        }
        public void Set<T>(string key, T t, TimeSpan expiry)
        {
            _database.StringSet(key, JsonConvert.SerializeObject(t), expiry);
        }

        public T Get<T>(string key)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(_database.StringGet(key));
                return result;
            }
            catch
            {
                return default(T);
            }
        }

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

        public IList<string> GetAllKeysByScript(string pattern)
        {
            var keys = new List<string>();
            var redisResult = _database.ScriptEvaluateAsync(LuaScript.Prepare(@" local res = redis.call('KEYS', @keypattern) " + " return res "), new { @keypattern = pattern + "*" }).GetAwaiter().GetResult();
            if (!redisResult.IsNull)
                keys.AddRange((string[])redisResult);

            return keys;

        }

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

        public async Task<bool> LockAsync(string key, TimeSpan expiry, CommandFlags flags = CommandFlags.None, string value = "1")
        {
            return await _database.LockTakeAsync(key, value, expiry, flags);
        }

        public async Task UnLockAsync(string key, CommandFlags flags = CommandFlags.None, string value = "1")
        {
            await _database.LockReleaseAsync(key, value, flags);
        }



        public bool ContainsKey(string key)
        {
            return _database.KeyExists(key);
        }

        public void Remove(string key)
        {
            _database.KeyDelete(key);
        }

        public void Start()
        {
            Connect();
        }



        public void Dispose()
        {
            _log.Warn($"RedisClient Disposed, EndPoints[{_options.EndPoint}], DatabaseNo[{_options.DatabaseNo}]");
            _cacheServer.CloseAsync();
            _cacheServer.Dispose();
        }
    }
}
