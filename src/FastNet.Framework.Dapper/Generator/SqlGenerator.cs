using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FastNet.Framework.Dapper.Mapper;
using FastNet.Framework.Dapper.Utils;

namespace FastNet.Framework.Dapper.Generator
{
    /// <summary>
    /// sql语句构造器
    /// </summary>
    public class SqlGenerator : ISqlGenerator
    {
        /// <summary>
        /// 参数前缀
        /// </summary>
        public virtual char ParameterPrefix
        {
            get { return '@'; }
        }

        /// <summary>
        /// 空的表达式
        /// </summary>
        public virtual string EmptyExpression
        {
            get { return "1=1"; }
        }

        /// <summary>
        /// 返回自增的sql语句
        /// </summary>
        public virtual string SelectIdentitySql
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 获取类映射
        /// </summary>
        /// <param name="t">类型</param>
        /// <returns></returns>
        public virtual ClassMapper GetMapper(Type t)
        {
            List<PropertyInfo> propertys = null;
            TableAttribute attribute = null;
            propertys = ReflectionUtils.TypePropertiesCache(t);
            attribute = ReflectionUtils.CustomAttributesCache(t);
            ClassMapper map = new ClassMapper
            {
                TableName = attribute == null ? "" : attribute.TableName,
                PrimaryKey = attribute == null ? "" : attribute.PrimaryKey,
                PrimaryKeyType = attribute == null ? PrimaryKeyType.Assigned : attribute.PrimaryKeyType,
                Properties = propertys
            };
            return map;
        }

        /// <summary>
        /// Insert语句
        /// </summary>
        /// <returns></returns>
        public virtual string GetInsertSql<T>()
        {
            Type t = typeof(T);
            ClassMapper map = GetMapper(t);
            string columnNames = string.Empty;
            string parameters = string.Empty;
            string selectIdentitySql = string.Empty;
            if (map.PrimaryKeyType == PrimaryKeyType.Identity)
            {
                columnNames = map.Properties.Where(p => p.Name.ToLower() != map.PrimaryKey.ToLower())
                    .Select(p => p.Name).AppendStrings(",");
                parameters = map.Properties.Where(p => p.Name.ToLower() != map.PrimaryKey.ToLower())
                    .Select(p => ParameterPrefix + p.Name).AppendStrings(",");
                selectIdentitySql = SelectIdentitySql;
            }
            else
            {
                columnNames = map.Properties.Select(p => p.Name).AppendStrings(",");
                parameters = map.Properties.Select(p => ParameterPrefix + p.Name).AppendStrings(",");
            }
            return string.Format("INSERT INTO {0} ({1}) values ({2});{3}", map.TableName, columnNames, parameters, selectIdentitySql);
        }

        /// <summary>
        /// Select语句
        /// </summary>
        /// <returns></returns>
        public virtual string GetSelectSql<T>()
        {
            ClassMapper mapT = GetMapper(typeof(T));
            return string.Format("SELECT * FROM {0} ", mapT.TableName);
        }

        /// <summary>
        /// Select语句
        /// </summary>
        /// <returns></returns>
        public virtual string GetSelectSql<T>(object param)
        {
            ClassMapper mapT = GetMapper(typeof(T));
            Type type = param.GetType();
            string strWhere = type.GetProperties().Select(p => string.Format("{0}={1}{0}", p.Name, ParameterPrefix)).AppendStrings(" and ");
            return string.Format("SELECT * FROM {0} WHERE {1}", mapT.TableName, strWhere);
        }

        /// <summary>
        /// Update语句(默认更新条件为主键)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string GetUpdateSql<T>(T t)
        {
            ClassMapper mapT = GetMapper(typeof(T));
            string strSet = mapT.Properties.Where(p => p.Name.ToLower() != mapT.PrimaryKey.ToLower() && p.GetValue(t, null) != null)
                .Select(p => string.Format("{0}={1}{0}", p.Name, ParameterPrefix)).AppendStrings(",");
            string strWhere = string.Format("{0}={1}{0}", mapT.PrimaryKey, ParameterPrefix);
            return string.Format("UPDATE {0} SET {1} WHERE {2}", mapT.TableName, strSet, strWhere);
        }

        /// <summary>
        /// Update语句(指定条件)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string GetUpdateSql<T>(T t, object param)
        {
            ClassMapper mapT = GetMapper(typeof(T));
            string set = mapT.Properties.Where(p => p.Name.ToLower() != mapT.PrimaryKey.ToLower() && p.GetValue(t, null) != null)
                .Select(p => string.Format("{0}={1}{0}", p.Name, ParameterPrefix)).AppendStrings(",");

            Type type = param.GetType();
            string strWhere = type.GetProperties().Select(p => string.Format("{0}={1}{0}", p.Name, ParameterPrefix)).AppendStrings(" and ");
            return string.Format("UPDATE {0} SET {1} WHERE {2}", mapT.TableName, set, strWhere);
        }

        /// <summary>
        /// Delete语句(全表)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual string GetDeleteSql<T>()
        {
            ClassMapper mapT = GetMapper(typeof(T));
            return string.Format("DELETE FROM {0} ", mapT.TableName);
        }

        /// <summary>
        /// Delete语句(指定条件)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual string GetDeleteSql<T>(object param)
        {
            ClassMapper mapT = GetMapper(typeof(T));
            Type type = param.GetType();
            string strWhere = type.GetProperties().Select(p => string.Format("{0}={1}{0}", p.Name, ParameterPrefix)).AppendStrings(" and ");
            return string.Format("DELETE FROM {0} WHERE {1}", mapT.TableName, strWhere);
        }

        /// <summary>
        /// Count语句(全表)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual string GetCountSql<T>()
        {
            ClassMapper mapT = GetMapper(typeof(T));
            return string.Format("SELECT COUNT(1) FROM {0}", mapT.TableName);
        }

        /// <summary>
        /// Count语句(指定条件)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual string GetCountSql<T>(object param)
        {
            ClassMapper mapT = GetMapper(typeof(T));
            Type type = param.GetType();
            string strWhere = type.GetProperties().Select(p => string.Format("{0}={1}{0}", p.Name, ParameterPrefix)).AppendStrings(" and ");
            return string.Format("SELECT COUNT(1) FROM {0} WHERE {1}", mapT.TableName, strWhere);
        }

        /// <summary>
        /// 分页语句（全表）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public virtual string GetPageListSql<T>(int pageIndex, int pageSize, string orderBy)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 分页语句(指定条件)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W">查询对象</typeparam>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public virtual string GetPageListSql<T>(object param, int pageIndex, int pageSize, string orderBy)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 分页语句(联表查询)
        /// </summary>
        /// <param name="sql">传入的联表查询语句</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public virtual string GetPageListSql<T>(string sql, int pageIndex, int pageSize, string orderBy)
        {
            throw new NotImplementedException();
        }
    }
}
