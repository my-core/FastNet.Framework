using FastNet.Framework.Dapper.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.Dapper
{
    public class QueryCondition
    {
        Dictionary<ConditionType, string> _condition = new Dictionary<ConditionType, string>();
        public QueryCondition()
        {

        }

        public void Add(ConditionType conditionType, string fieldName)
        {
            _condition.Add(conditionType, fieldName);
        }

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
