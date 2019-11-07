using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FastNet.Framework.Mongo
{
    public class MongoRepository : IMongoRepository
    {
        public IMongoDatabase database = null;
        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="connectionStringName"></param>
        public MongoRepository(string connString, string dbName)
        {
            MongoClient client = new MongoClient(connString);
            database = client.GetDatabase(dbName);
        }

        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="connectionStringName"></param>
        public MongoRepository(MongoOptions mongoOptions)
        {
            MongoClient client = new MongoClient(mongoOptions.Host);
            database = client.GetDatabase(mongoOptions.DatabaseName);
        }

        /// <summary>
        /// 插入单条数据
        /// </summary>
        /// <param name="t"></param>
        public void Insert<T>(T t)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            collection.InsertOne(t);
        }
        /// <summary>
        /// 插入单条数据(异步)
        /// </summary>
        /// <param name="t"></param>
        public void InsertAsync<T>(T t)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            collection.InsertOneAsync(t);
        }

        /// <summary>
        /// 插入多条数据
        /// </summary>
        /// <param name="list"></param>
        public void Insert<T>(List<T> list)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            collection.InsertMany(list);
        }
        /// <summary>
        /// 插入多条数据(异步)
        /// </summary>
        /// <param name="list"></param>
        public void InsertAsync<T>(List<T> list)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            collection.InsertManyAsync(list);
        }
        /// <summary>
        /// 删除所有数据
        /// </summary>
        public void Delete<T>()
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var filter = new BsonDocument();
            collection.DeleteMany(filter);
        }
        /// <summary>
        /// 删除所有数据(异步)
        /// </summary>
        public void DeleteAsync<T>()
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var filter = new BsonDocument();
            collection.DeleteManyAsync(filter);
        }
        /// <summary>
        /// 删除指定条件的数据
        /// </summary>
        public void Delete<T>(string key, object value)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq(key, value);
            collection.DeleteMany(filter);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public List<T> List<T>()
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var filter = new BsonDocument();
            return collection.Find(filter).ToList();
        }
        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> ListAsync<T>()
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var filter = new BsonDocument();
            return await collection.Find(filter).ToListAsync();
        }
        /// <summary>
        /// 查询数据（默认-条件与）
        /// </summary>
        /// <returns></returns>
        public List<T> List<T>(object param)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Empty;
            if (param != null)
            {
                List<PropertyInfo> propertyInfos = param.GetType().GetProperties().ToList();
                foreach (PropertyInfo p in propertyInfos)
                {
                    filter &= builder.Eq(p.Name, p.GetValue(param));
                }
            }
            return collection.Find(filter).ToList();
        }
        /// <summary>
        /// 查询数据（默认-条件与）
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> ListAsync<T>(object param)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Empty;
            if (param != null)
            {
                List<PropertyInfo> propertyInfos = param.GetType().GetProperties().ToList();
                foreach (PropertyInfo p in propertyInfos)
                {
                    filter &= builder.Eq(p.Name, p.GetValue(param));
                }
            }
            return await collection.Find(filter).ToListAsync();
        }
        /// <summary>
        /// 查询数据（条件或）
        /// </summary>
        /// <returns></returns>
        public List<T> ListOr<T>(object param)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Empty;
            if (param != null)
            {
                List<PropertyInfo> propertyInfos = param.GetType().GetProperties().ToList();
                foreach (PropertyInfo p in propertyInfos)
                {
                    filter |= builder.Eq(p.Name, p.GetValue(param));
                }
            }
            return collection.Find(filter).ToList();
        }/// <summary>
         /// 查询数据（条件或）
         /// </summary>
         /// <returns></returns>
        public async Task<List<T>> ListOrAsync<T>(object param)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Empty;
            if (param != null)
            {
                List<PropertyInfo> propertyInfos = param.GetType().GetProperties().ToList();
                foreach (PropertyInfo p in propertyInfos)
                {
                    filter |= builder.Eq(p.Name, p.GetValue(param));
                }
            }
            return await collection.Find(filter).ToListAsync();
        }
        /// <summary>
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="hs">条件</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sortFiled">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public List<T> GetPagedList<T>(object param, int pageIndex, int pageSize, string sortFiled, bool isAsc)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Empty;
            if (param != null)
            {
                List<PropertyInfo> propertyInfos = param.GetType().GetProperties().ToList();
                foreach (PropertyInfo p in propertyInfos)
                {
                    filter &= builder.Eq(p.Name, p.GetValue(param));
                }
            }
            var sort = isAsc ? Builders<T>.Sort.Ascending(sortFiled) : Builders<T>.Sort.Descending(sortFiled);
            return collection.Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
        }

        /// <summary>
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="hs">条件</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sortFiled">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public async Task<List<T>> GetPagedListAsync<T>(object param, int pageIndex, int pageSize, string sortFiled, bool isAsc)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Empty;
            if (param != null)
            {
                List<PropertyInfo> propertyInfos = param.GetType().GetProperties().ToList();
                foreach (PropertyInfo p in propertyInfos)
                {
                    filter &= builder.Eq(p.Name, p.GetValue(param));
                }
            }
            var sort = isAsc ? Builders<T>.Sort.Ascending(sortFiled) : Builders<T>.Sort.Descending(sortFiled);
            return await collection.Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
        }
    }
}
