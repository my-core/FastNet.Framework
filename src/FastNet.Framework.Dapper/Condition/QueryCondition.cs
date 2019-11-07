using FastNet.Framework.Dapper.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.Dapper
{
    /// <summary>
    /// dapper查询条件扩展
    /// </summary>
    public class QueryCondition
    {
        /// <summary>
        /// 条件集
        /// </summary>
        Dictionary<ConditionType, string> _condition = new Dictionary<ConditionType, string>();
        public QueryCondition()
        {

        }
        /// <summary>
        /// 添加条件
        /// </summary>
        /// <param name="conditionType"></param>
        /// <param name="fieldName"></param>
        public void Add(ConditionType conditionType, string fieldName)
        {
            _condition.Add(conditionType, fieldName);
        }
        /// <summary>
        /// 转换为'and'连接成的sql语句
        /// </summary>
        /// <returns></returns>
        public string ToSqlWithAnd()
        {
            List<string> query = new List<string>();
            foreach (ConditionType key in _condition.Keys)
            {
                switch (key)
                {
                    case ConditionType.Equal:
                        query.Add(string.Format("{0} = @{0}", _condition[key]));
                        break;
                    case ConditionType.GreaterThan:
                        query.Add(string.Format("{0} > @{0}", _condition[key]));
                        break;
                    case ConditionType.GreaterThanOrEqual:
                        query.Add(string.Format("{0} >= @{0}", _condition[key]));
                        break;
                    case ConditionType.LessThan:
                        query.Add(string.Format("{0} < @{0}", _condition[key]));
                        break;
                    case ConditionType.LessThanOrEqual:
                        query.Add(string.Format("{0} <= @{0}", _condition[key]));
                        break;
                    case ConditionType.Like:
                        query.Add(string.Format("{0} like '%'+@{0}+'%'", _condition[key]));
                        break;
                    default: break;
                }
            }
            return query.AppendStrings(" and ");
        }
        /// <summary>
        /// 转换为'and'连接成的sql语句
        /// </summary>
        /// <returns></returns>
        public string ToSqlWithOr()
        {
            List<string> query = new List<string>();
            foreach (ConditionType key in _condition.Keys)
            {
                switch (key)
                {
                    case ConditionType.Equal:
                        query.Add(string.Format("{0} = @{0}", _condition[key]));
                        break;
                    case ConditionType.GreaterThan:
                        query.Add(string.Format("{0} > @{0}", _condition[key]));
                        break;
                    case ConditionType.GreaterThanOrEqual:
                        query.Add(string.Format("{0} >= @{0}", _condition[key]));
                        break;
                    case ConditionType.LessThan:
                        query.Add(string.Format("{0} < @{0}", _condition[key]));
                        break;
                    case ConditionType.LessThanOrEqual:
                        query.Add(string.Format("{0} <= @{0}", _condition[key]));
                        break;
                    case ConditionType.Like:
                        query.Add(string.Format("{0} like '%'+@{0}+'%'", _condition[key]));
                        break;
                    default: break;
                }
            }
            return query.AppendStrings(" or ");
        }
    }
}
