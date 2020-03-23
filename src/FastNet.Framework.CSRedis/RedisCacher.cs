using CSRedis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastNet.Framework.CSRedis
{
    /// <summary>
    /// Redis缓存器基类
    /// </summary>
    public class RedisCacher : IRedisCacher
    {
        private RedisOptions _options;
        private CSRedisClient _client;

        public RedisCacher(RedisOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// 连接redis
        /// </summary>
        public void Connect()
        {
            if (_options.IsSentinelModel == 1)
            {
                _client = new CSRedisClient(_options.MasterName, _options.Sentinels);
            }
            else
            {
                _client = new CSRedisClient(_options.ConnectionString);
            }           
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
            _client.Set(key, JsonConvert.SerializeObject(t));
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
            _client.Set(key, JsonConvert.SerializeObject(t), (int)expiry.TotalSeconds);
        }

        /// <summary>
        /// 获取key缓存String类型数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (ContainsKey(key))
            {
                var result = JsonConvert.DeserializeObject<T>(_client.Get(key));
                return result;
            }
            return default(T);
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
            if (ContainsKey(key))
            {
                var value = _client.GetSet(key, JsonConvert.SerializeObject(newValue));
                if (!string.IsNullOrEmpty(value))
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
            }
            return default(T);
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
            _client.HSet(key, hashField, JsonConvert.SerializeObject(t));
        }

        /// <summary>
        /// 设置Key缓存Hash类型数据(批量)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="ts"></param>
        public void SetHash<T>(string key, IDictionary<string, T> ts)
        {
            List<string> param = new List<string>();
            foreach (var item in ts)
            {
                param.Add(item.Key);
                param.Add(JsonConvert.SerializeObject(item.Value));
            }
            _client.HMSet(key, param.ToArray());
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
            return _client.HIncrBy(key, hashField, incrementStep);
        }
       
        /// <summary>
        /// 获取Key缓存数据(全部)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetHash<T>(string key)
        {
            var result = new List<T>();
            var hashValues = _client.HVals(key);
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
            var value = _client.HGet(key, hashField);
            if (!string.IsNullOrEmpty(value))
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
            var values = _client.HMGet(key, hashFields.Select(a => a).ToArray());
            if (values != null && values.Count() > 0)
            {
                foreach (var value in values)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        result.Add(JsonConvert.DeserializeObject<T>(value));
                    }
                }
            }
            return result;
        }

        #endregion

        #region Set
        /// <summary>
        /// 判断ZSet中某个键是否存在指定成员
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool SetContains(string key, string item)
        {
            return _client.SIsMember(key, item);
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
            return _client.Exists(key);
        }       

        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            _client.Del(key);
        }

        #endregion

        #region Redis Lock
        /// <summary>
        /// 对key加锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireSecond"></param>
        /// <returns></returns>
        public bool LockAsync(string key, int expireSecond)
        {
            if (_client.SetNx(key, (DateTime.Now.Ticks / 10000) + expireSecond * 1000))
            {
                _client.Expire(key, expireSecond);
                return true;
            }
            else
            {
                var value = _client.Get(key);
                if (!string.IsNullOrEmpty(value))
                {
                    long lastValue = 0;
                    if (long.TryParse(value, out lastValue))
                    {
                        if (lastValue > DateTime.Now.Ticks / 10000)
                            return false;
                        else
                        {
                            _client.Del(key);
                            return false;
                        }
                    }
                    else
                    {
                        _client.Del(key);
                        return false;
                    }
                }
                else
                {
                    _client.Del(key);
                    return false;
                }
            }
        }
        /// <summary>
        /// 对key释放锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ReleaseLock(string key)
        {
            try
            {
                return _client.Del(key) > 0;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
