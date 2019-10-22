using FastNet.Framework.Dapper.Mapper;
using FastNet.Framework.Dapper.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNet.Framework.Dapper.Generator
{
    public class MySqlGenerator : SqlGenerator, ISqlGenerator
    {
        public override string SelectIdentitySql
        {
            get { return "SELECT LAST_INSERT_ID();"; }
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
            ClassMapper mapT = GetMapper(typeof(T));
            return string.Format("SELECT * FROM {0} order by {1} LIMIT {2},{3}", mapT.TableName, string.IsNullOrEmpty(orderBy) ? mapT.PrimaryKey : orderBy, (pageIndex - 1) * pageSize, pageSize);
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
            ClassMapper mapT = GetMapper(typeof(T));
            Type type = param.GetType();
            string strWhere = type.GetProperties().Select(p => string.Format("{0}={1}{0}", p.Name, ParameterPrefix)).AppendStrings(" and ");
            return string.Format("SELECT * FROM {0} WHERE {1} ORDER BY {2} LIMIT {3},{4}", mapT.TableName, string.IsNullOrEmpty(strWhere) ? EmptyExpression : strWhere, string.IsNullOrEmpty(orderBy) ? mapT.PrimaryKey : orderBy, (pageIndex - 1) * pageSize, pageSize);
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
            //ClassMapper mapT = GetMapper(typeof(T));
            StringBuilder sbSql = new StringBuilder(sql);
            if (!string.IsNullOrEmpty(orderBy))
            {
                sbSql.Append(string.Format(" ORDER BY {0}",  orderBy));
            }
            sbSql.Append(string.Format(" LIMIT {0},{1}", (pageIndex - 1) * pageSize, pageSize));
            return sbSql.ToString();
        }
    }
}
