using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastNet.Framework.Dapper
{
    /// <summary>
    /// 通用数据库操作接口
    /// </summary>
    public interface IBaseRepository
    {
        /// <summary>
        ///  OpenDbConnection
        /// </summary>
        /// <returns></returns>
        IDbConnection OpenConnection();

        #region ---Insert---
        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        long Insert<T>(T t);

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        int Insert<T>(List<T> listT);
        #endregion

        #region ---Update---

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        int Update<T>(T t);
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        int Update<T>(T t,object param);
        #endregion

        #region ---Delete---
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        int Delete<T>();
        
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        int Delete<T>(object param);
        #endregion

        #region ---GetModel---
        /// <summary>
        /// 查询(单记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        T GetModel<T>(object param);
        #endregion

        #region ---GetList---
        /// <summary>
        /// 查询(多记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetList<T>();

        /// <summary>
        /// 查询(多记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        List<T> GetList<T>(object param);
        /// <summary>
        /// 查询(多记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W"></typeparam>
        /// <param name="sql"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        List<T> GetList<T>(string sql, object param);
        #endregion

        #region ---GetPageList---
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        PagedList<T> GetPagedList<T>(int pageIndex, int pageSize, string orderBy);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql查询语句</param>
        /// <param name="param">sql参数</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        PagedList<T> GetPagedList<T>(object param, int pageIndex, int pageSize, string orderBy);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql查询语句</param>
        /// <param name="param">sql参数</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        PagedList<T> GetPagedList<T>(string sql, object param, int pageIndex, int pageSize, string orderBy);
        #endregion

        #region ---GetCount---
        /// <summary>
        /// 获取计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        int GetCount<T>();
        /// <summary>
        /// 获取计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        int GetCount<T>(object  param);
        #endregion

        #region ---InsertAsync---
        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns>返回自增</returns>
        Task<long> InsertAsync<T>(T t);

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<int> InsertAsync<T>(List<T> listT);
        #endregion

        #region ---UpdateAsync---
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(T t);
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(T t, object param);
        #endregion

        #region ---DeleteAsync---
        /// <summary>
        /// 全表删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<int> DeleteAsync<T>();

        /// <summary>
        /// 按指定条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        Task<int> DeleteAsync<T>(object param);
        #endregion

        #region ---GetModelAsync---
        /// <summary>
        /// 按指定条件查询(单记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        Task<T> GetModelAsync<T>(object param);
        #endregion

        #region ---GetListAsync---

        /// <summary>
        /// 全表查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<List<T>> GetListAsync<T>();

        /// <summary>
        /// 按指定条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        Task<List<T>> GetListAsync<T>(object param);

        /// <summary>
        /// 指定条件联表查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W"></typeparam>
        /// <param name="sql"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<List<T>> GetListAsync<T>(string sql, object param);
        #endregion

        #region ---GetPageListAsync---

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        Task<PagedList<T>> GetPageListAsync<T>(int pageIndex, int pageSize, string orderBy);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql查询语句</param>
        /// <param name="param">sql参数</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        Task<PagedList<T>> GetPagedListAsync<T>(object param, int pageIndex, int pageSize, string orderBy);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql查询语句</param>
        /// <param name="param">sql参数</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        Task<PagedList<T>> GetPagedListAsync<T>(string sql, object param, int pageIndex, int pageSize, string orderBy);
        #endregion

        #region ---GetCountAsync---

        /// <summary>
        /// 全表查询计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<int> GetCountAsync<T>();
        /// <summary>
        /// 按指定条件查询计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<int> GetCountAsync<T>(object param);
        #endregion
    }
}
