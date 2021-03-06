using FastNet.Framework.Dapper.Mapper;
using FastNet.Framework.Dapper.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNet.Framework.Dapper.Generator
{
    public class SqlServerGenerator : SqlGenerator, ISqlGenerator
    {
        /// <summary>
        /// 参数前缀
        /// </summary>
        public override char ParameterPrefix
        {
            get { return '@'; }
        }

        /// <summary>
        /// 返回自增的sql语句
        /// </summary>
        public override string SelectIdentitySql
        {
            get { return " select @@identity; "; }
        }
        /// <summary>
        /// 分页语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public override string GetPageListSql<T>(int pageIndex, int pageSize, string orderBy)
        {
            string PageSql = @"select * from (
	                                    select *, ROW_NUMBER() OVER(Order by {0} ) AS RowId from {1} where {2}
                                    ) as b where RowId between {3} and {4} ";
            ClassMapper mapT = GetMapper(typeof(T));
            return string.Format(PageSql, orderBy, mapT.TableName, "", (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
        }

        /// <summary>
        /// 分页语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W">查询对象</typeparam>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public override string GetPageListSql<T>(object param, int pageIndex, int pageSize, string orderBy)
        {
            string PageSql = @"select * from (
	                                    select *, ROW_NUMBER() OVER(Order by {0} ) AS RowId from {1} where {2}
                                    ) as b where RowId between {3} and {4} ";
            ClassMapper mapT = GetMapper(typeof(T));

            Type type = param.GetType();
            string strWhere = type.GetProperties().Select(p => string.Format("{0}={1}{0}", p.Name, ParameterPrefix)).AppendStrings(" and ");
            return string.Format(PageSql, orderBy, mapT.TableName, strWhere, orderBy, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
        }
        /// <summary>
        /// 分页语句(联表查询)
        /// </summary>
        /// <param name="sql">传入的联表查询语句</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public override string GetPageListSql<T>(string sql, int pageIndex, int pageSize, string orderBy)
        {
            string PageSql = @"select * from (
	                                    select *, ROW_NUMBER() OVER(Order by {0} ) AS RowId from ({1}) as A
                                    ) as B where RowId between {2} and {3} ";
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(string.Format(PageSql, orderBy, sql, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize));
            return sbSql.ToString();
        }
    }
}
