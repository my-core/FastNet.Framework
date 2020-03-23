using Dapper;
using FastNet.Framework.Dapper.Generator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace FastNet.Framework.Dapper
{
    public abstract class BaseRepository : IBaseRepository
    {
        /// <summary>
        /// 数据库连接串
        /// </summary>
        private string _connString { get; set; }
        /// <summary>
        /// sql语句构造器
        /// </summary>
        private ISqlGenerator _sqlGenerator { get; set; }
        public BaseRepository(string connString, ISqlGenerator sqlGenerator)
        {
            _connString = connString;
            _sqlGenerator = sqlGenerator;
        }

        #region ---Insert---
        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns>返回自增</returns>
        public virtual long Insert<T>(T t)
        {
            try
            {
                string sql = _sqlGenerator.GetInsertSql<T>();
                using(var conn = OpenConnection())
                {
                    return Convert.ToInt64(conn.ExecuteScalar(sql, t));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual int Insert<T>(List<T> listT)
        {
            try
            {
                string sql = _sqlGenerator.GetInsertSql<T>();
                using (var conn = OpenConnection())
                {
                    return conn.Execute(sql, listT);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ---Update---
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual int Update<T>(T t)
        {
            try
            {
                string sql = _sqlGenerator.GetUpdateSql<T>(t);
                using (var conn = OpenConnection())
                {
                    return conn.Execute(sql, t);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual int Update<T>(T t,object param)
        {
            try
            {
                string sql = _sqlGenerator.GetUpdateSql<T>(t, param);
                using (var conn = OpenConnection())
                {
                    return conn.Execute(sql, param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ---Delete---
        /// <summary>
        /// 全表删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual int Delete<T>()
        {
            try
            {
                string sql = _sqlGenerator.GetDeleteSql<T>();
                using (var conn = OpenConnection())
                {
                    return conn.Execute(sql);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// 按指定条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public int Delete<T>(object param)
        {
            try
            {
                string sql = _sqlGenerator.GetDeleteSql<T>(param);
                using (var conn = OpenConnection())
                {
                    return conn.Execute(sql, param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ---GetModel---
        /// <summary>
        /// 按指定条件查询(单记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public virtual T GetModel<T>(object param)
        {
            try
            {
                string sql = _sqlGenerator.GetSelectSql<T>(param);
                using (var conn = OpenConnection())
                {
                    return conn.QuerySingleOrDefault<T>(sql, param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ---GetList---

        /// <summary>
        /// 全表查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> GetList<T>()
        {
            try
            {
                string sql = _sqlGenerator.GetSelectSql<T>();
                using (var conn = OpenConnection())
                {
                    return conn.Query<T>(sql).AsList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 按指定条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(object param)
        {
            try
            {
                string sql = _sqlGenerator.GetSelectSql<T>(param);
                using (var conn = OpenConnection())
                {
                    return conn.Query<T>(sql, param).AsList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 指定条件联表查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W"></typeparam>
        /// <param name="sql"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string sql, object param)
        {
            try
            {
                using (var conn = OpenConnection())
                {
                    return conn.Query<T>(sql, param).AsList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
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
        public virtual PagedList<T> GetPagedList<T>(int pageIndex, int pageSize, string orderBy)
        {
            try
            {
                string sql = _sqlGenerator.GetPageListSql<T>(pageIndex, pageSize, orderBy);
                using (var conn = OpenConnection())
                {
                    int totalCount = GetCount<T>();
                    List<T> list = conn.Query<T>(sql).AsList();
                    return new PagedList<T>(list, pageIndex, pageSize, totalCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
        public virtual PagedList<T> GetPagedList<T>(object param, int pageIndex, int pageSize, string orderBy)
        {
            try
            {
                string sql = _sqlGenerator.GetPageListSql<T>(param, pageIndex, pageSize, orderBy);
                using (var conn = OpenConnection())
                {
                    int totalCount = GetCount<T>(param);
                    List<T> list = conn.Query<T>(sql, param).AsList();
                    return new PagedList<T>(list, pageIndex, pageSize, totalCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
        public virtual PagedList<T> GetPagedList<T>(string sql, object param, int pageIndex, int pageSize, string orderBy)
        {
            try
            {
                string getCountSql = string.Format("select count(1) from ({0}) as A", sql);
                using (var conn = OpenConnection())
                {
                    int totalCount = Convert.ToInt32(conn.ExecuteScalar(getCountSql, param));
                    sql = _sqlGenerator.GetPageListSql<T>(sql, pageIndex, pageSize, orderBy);
                    List<T> list = conn.Query<T>(sql, param).AsList();
                    return new PagedList<T>(list, pageIndex, pageSize, totalCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ---GetCount---

        /// <summary>
        /// 全表查询计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual int GetCount<T>()
        {
            try
            {
                string sql = _sqlGenerator.GetCountSql<T>();
                using (var conn = OpenConnection())
                {
                    return Convert.ToInt32(conn.ExecuteScalar(sql));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 按指定条件查询计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int GetCount<T>(object param)
        {
            try
            {
                string sql = _sqlGenerator.GetCountSql<T>(param);
                using (var conn = OpenConnection())
                {
                    return Convert.ToInt32(conn.ExecuteScalar(sql, param));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ---InsertAsync---
        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns>返回自增</returns>
        public virtual async Task<long> InsertAsync<T>(T t)
        {
            try
            {
                string sql = _sqlGenerator.GetInsertSql<T>();
                using (var conn = OpenConnection())
                {
                    var result = await conn.ExecuteScalarAsync(sql, t);
                    return Convert.ToInt64(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync<T>(List<T> listT)
        {
            try
            {
                string sql = _sqlGenerator.GetInsertSql<T>();
                using (var conn = OpenConnection())
                {
                    return await conn.ExecuteAsync(sql, listT);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ---UpdateAsync---
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync<T>(T t)
        {
            try
            {
                string sql = _sqlGenerator.GetUpdateSql<T>(t);
                using (var conn = OpenConnection())
                {
                    return await conn.ExecuteAsync(sql, t);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync<T>(T t, object param)
        {
            try
            {
                string sql = _sqlGenerator.GetUpdateSql<T>(t, param);
                using (var conn = OpenConnection())
                {
                    return await conn.ExecuteAsync(sql, param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ---DeleteAsync---
        /// <summary>
        /// 全表删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual async Task<int> DeleteAsync<T>()
        {
            try
            {
                string sql = _sqlGenerator.GetDeleteSql<T>();
                using (var conn = OpenConnection())
                {
                    return await conn.ExecuteAsync(sql);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 按指定条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync<T>(object param)
        {
            try
            {
                string sql = _sqlGenerator.GetDeleteSql<T>(param);
                using (var conn = OpenConnection())
                {
                    return await conn.ExecuteAsync(sql, param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ---GetModelAsync---
        /// <summary>
        /// 按指定条件查询(单记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public virtual async Task<T> GetModelAsync<T>(object param)
        {
            try
            {
                string sql = _sqlGenerator.GetSelectSql<T>(param);
                using (var conn = OpenConnection())
                {
                    return await conn.QuerySingleOrDefaultAsync<T>(sql, param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ---GetListAsync---

        /// <summary>
        /// 全表查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync<T>()
        {
            try
            {
                string sql = _sqlGenerator.GetSelectSql<T>();
                using (var conn = OpenConnection())
                {
                    var list= await conn.QueryAsync<T>(sql);
                    return list.AsList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 按指定条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hs"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync<T>(object param)
        {
            try
            {
                string sql = _sqlGenerator.GetSelectSql<T>(param);
                using (var conn = OpenConnection())
                {
                    var list = await conn.QueryAsync<T>(sql, param);
                    return list.AsList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 指定条件联表查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W"></typeparam>
        /// <param name="sql"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync<T>(string sql, object param)
        {
            try
            {
                using (var conn = OpenConnection())
                {
                    var list = await conn.QueryAsync<T>(sql, param);
                    return list.AsList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
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
        public virtual async Task<PagedList<T>> GetPageListAsync<T>(int pageIndex, int pageSize, string orderBy)
        {
            try
            {
                string sqlCount = _sqlGenerator.GetCountSql<T>();
                string sqlList = _sqlGenerator.GetPageListSql<T>(pageIndex, pageSize, orderBy);
                string sql = sqlCount + ";" + sqlList;
                using (var conn = OpenConnection())
                {
                    var multi = await conn.QueryMultipleAsync(sql);
                    var totalCount = multi.Read<int>().AsList().FirstOrDefault();
                    List<T> list = multi.Read<T>().AsList();
                    return new PagedList<T>(list, pageIndex, pageSize, totalCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
        public virtual async Task<PagedList<T>> GetPagedListAsync<T>(object param, int pageIndex, int pageSize, string orderBy)
        {
            try
            {
                string sqlCount = _sqlGenerator.GetCountSql<T>(param);
                string sqlList = _sqlGenerator.GetPageListSql<T>(param, pageIndex, pageSize, orderBy);
                string sql = sqlCount + ";" + sqlList;
                using (var conn = OpenConnection())
                {
                    var multi = await conn.QueryMultipleAsync(sql,param);
                    var totalCount = multi.Read<int>().AsList().FirstOrDefault();
                    List<T> list = multi.Read<T>().AsList();
                    return new PagedList<T>(list, pageIndex, pageSize, totalCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
        public virtual async Task<PagedList<T>> GetPagedListAsync<T>(string sql, object param, int pageIndex, int pageSize, string orderBy)
        {
            try
            {
                string sqlCount = string.Format("select count(1) from ({0}) as A;", sql);
                string sqlList = _sqlGenerator.GetPageListSql<T>(sql, pageIndex, pageSize, orderBy);
                sql = sqlCount + sqlList;
                using (var conn = OpenConnection())
                {
                    var multi = await conn.QueryMultipleAsync(sql, param);
                    var totalCount = multi.Read<int>().AsList().FirstOrDefault();
                    List<T> list = multi.Read<T>().AsList();
                    return new PagedList<T>(list, pageIndex, pageSize, totalCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ---GetCountAsync---

        /// <summary>
        /// 全表查询计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual async Task<int> GetCountAsync<T>()
        {
            try
            {
                string sql = _sqlGenerator.GetCountSql<T>();
                using (var conn = OpenConnection())
                {
                    var result = await conn.ExecuteScalarAsync(sql);
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 按指定条件查询计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<int> GetCountAsync<T>(object param)
        {
            try
            {
                string sql = _sqlGenerator.GetCountSql<T>(param);
                using (var conn = OpenConnection())
                {
                    var result = await conn.ExecuteScalarAsync(sql, param);
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ---ExecuteSql---
        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, null);
        }
        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(string commandText, params DbParameter[] parms)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, object parms)
        {
            using (var conn = OpenConnection())
            {
                return conn.Execute(commandText, parms, null, null, commandType);
            }
        }
        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(CommandType.Text, commandText, null);
        }
        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(string commandText, object parms)
        {
            return ExecuteScalar(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(CommandType commandType, string commandText, object parms)
        {
            using (var conn = OpenConnection())
            {
                return conn.ExecuteScalar(commandText, parms, null, null, commandType);
            }
        }
        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行</returns>
        public DataRow ExecuteDataRow(CommandType commandType, string commandText, object parms)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 执行SQL语句,返回结果集中的第一个数据表
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一个数据表</returns>
        public DataTable ExecuteDataTable(CommandType commandType, string commandText, object parms)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集</returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText, object parms)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region---open connection---

        /// <summary>
        /// OpenDbConnection（不同数据库驱动需要重写该方法）
        /// </summary>
        /// <returns></returns>
        public abstract IDbConnection OpenConnection();
        #endregion

    }
}
