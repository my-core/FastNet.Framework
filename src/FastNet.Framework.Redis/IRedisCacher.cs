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
    public interface IRedisCacher
    {
        /// <summary>
        /// 连接redis
        /// </summary>
        void Connect();

        #region String Value
        /// <summary>
        /// 设置key缓存String类型数据（永不过期）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        void Set<T>(string key, T t);

        /// <summary>
        /// 设置key缓存String类型数据（有期效）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="expiry">到期时间</param>
        void Set<T>(string key, T t, TimeSpan expiry);

        /// <summary>
        /// 获取key缓存String类型数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 设置新数据并返回旧数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        T GetSet<T>(string key, T newValue);
        #endregion

        #region Hash Value        
        /// <summary>
        /// 设置Key缓存Hash类型数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="t"></param>
        void SetHash<T>(string key, string hashField, T t);

        /// <summary>
        /// 设置Key缓存Hash类型数据(批量)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="ts"></param>
        void SetHash<T>(string key, IDictionary<string, T> ts);

        /// <summary>
        /// 自增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="incrementStep"></param>
        /// <returns></returns>
        long HashIncrement(string key, string hashField, int incrementStep = 1);

        /// <summary>
        ///自减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="incrementStep"></param>
        /// <returns></returns>
        long HashDecrement(string key, string field, int incrementStep = 1);

        /// <summary>
        /// 获取Key缓存数据(全部)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> GetHash<T>(string key);

        /// <summary>
        /// 获取Key缓存数据(指定hashFiled)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        T GetHash<T>(string key, string hashField);

        /// <summary>
        /// 获取Key缓存数据(指定hashFiled)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashFields"></param>
        /// <returns></returns>
        List<T> GetHash<T>(string key, List<string> hashFields);

        #endregion

        #region Exist/Get/Remove Key
        /// <summary>
        /// 是否存在key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(string key);

        /// <summary>
        /// 获取指定通配的所有key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        IList<string> GetAllKeys(string pattern);

        /// <summary>
        /// 获取指定通配的所有key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        IList<string> GetAllKeysByScript(string pattern);

        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 删除指定通配的所有key
        /// </summary>
        /// <param name="pattern"></param>
        void RemoveAll(string pattern);

        /// <summary>
        /// 删除指定通配的所有key
        /// </summary>
        /// <param name="pattern"></param>
        void RemoveAllByScript(string pattern);

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
        Task<bool> LockAsync(string key, TimeSpan expiry, CommandFlags flags = CommandFlags.None, string value = "1");
        /// <summary>
        /// 对key释放锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task UnLockAsync(string key, CommandFlags flags = CommandFlags.None, string value = "1");
        #endregion
    }
}
