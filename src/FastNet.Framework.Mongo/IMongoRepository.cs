using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FastNet.Framework.Mongo
{
    public interface IMongoRepository
    {
        /// <summary>
        /// 插入单条数据
        /// </summary>
        /// <param name="t"></param>
        void Insert<T>(T t);
        /// <summary>
        /// 插入单条数据(异步)
        /// </summary>
        /// <param name="t"></param>
        void InsertAsync<T>(T t);

        /// <summary>
        /// 插入多条数据
        /// </summary>
        /// <param name="list"></param>
        void Insert<T>(List<T> list);
        /// <summary>
        /// 插入多条数据(异步)
        /// </summary>
        /// <param name="list"></param>
        void InsertAsync<T>(List<T> list);
        /// <summary>
        /// 删除所有数据
        /// </summary>
        void Delete<T>();
        /// <summary>
        /// 删除所有数据(异步)
        /// </summary>
        void DeleteAsync<T>();
        /// <summary>
        /// 删除指定条件的数据
        /// </summary>
        void Delete<T>(string key, object value);
        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        List<T> List<T>();
        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ListAsync<T>();
        /// <summary>
        /// 查询数据（默认-条件与）
        /// </summary>
        /// <returns></returns>
        List<T> List<T>(object param);
        /// <summary>
        /// 查询数据（默认-条件与）
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ListAsync<T>(object param);
        /// <summary>
        /// 查询数据（条件或）
        /// </summary>
        /// <returns></returns>
        List<T> ListOr<T>(object param);
        /// <summary>
        /// 查询数据（条件或）
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ListOrAsync<T>(object param);
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="hs">条件</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sortFiled">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        List<T> GetPagedList<T>(object param, int pageIndex, int pageSize, string sortFiled, bool isAsc);
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="hs">条件</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sortFiled">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        Task<List<T>> GetPagedListAsync<T>(object param, int pageIndex, int pageSize, string sortFiled, bool isAsc);
    }
}
