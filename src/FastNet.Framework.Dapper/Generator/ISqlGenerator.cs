using FastNet.Framework.Dapper.Mapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNet.Framework.Dapper.Generator
{
    /// <summary>
    /// sql语句构造器
    /// </summary>
    public interface ISqlGenerator
    {
        /// <summary>
        /// 参数前缀
        /// </summary>
        char ParameterPrefix { get; }

        /// <summary>
        /// 空的表达式
        /// </summary>
        string EmptyExpression { get; }

        /// <summary>
        /// 获取类映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ClassMapper GetMapper(Type t);

        /// <summary>
        /// Insert语句
        /// </summary>
        /// <returns></returns>
        string GetInsertSql<T>();

        /// <summary>
        /// Select语句
        /// </summary>
        /// <returns></returns>
        string GetSelectSql<T>();

        /// <summary>
        /// Select语句
        /// </summary>
        /// <returns></returns>
        string GetSelectSql<T>(object param);

        /// <summary>
        /// Update语句(更新条件为主键)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetUpdateSql<T>(T t);

        /// <summary>
        /// Update语句(更新条件为主键)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetUpdateSql<T>(T t,object param);

        /// <summary>
        /// Delete语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetDeleteSql<T>();

        /// <summary>
        /// Delete语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetDeleteSql<T>(object param);

        /// <summary>
        /// Count语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetCountSql<T>();

        /// <summary>
        /// Count语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        string GetCountSql<T>(object param);

        /// <summary>
        /// 分页语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns>返回两条sql,string[0]取查询总记录数，string[2]取分页数据</returns>
        string GetPageListSql<T>(int pageIndex, int pageSize, string orderBy);

        /// <summary>
        /// 分页语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W">查询对象</typeparam>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns>返回两条sql,string[0]取查询总记录数，string[2]取分页数据</returns>
        string GetPageListSql<T>(object param, int pageIndex, int pageSize, string orderBy);

        /// <summary>
        /// 分页语句(联表查询)
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns>返回两条sql,string[0]取查询总记录数，string[2]取分页数据</returns>
        string GetPageListSql<T>(string sql, int pageIndex, int pageSize, string orderBy);
    }
}
