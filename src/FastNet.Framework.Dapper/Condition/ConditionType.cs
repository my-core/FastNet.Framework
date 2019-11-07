using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.Dapper
{
    public enum ConditionType
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 大于 >
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 大于等于 >=
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// 模糊匹配
        /// </summary>
        Like
    }
}
