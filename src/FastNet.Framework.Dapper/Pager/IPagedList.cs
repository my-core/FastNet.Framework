using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastNet.Framework.Dapper
{
    public interface IPagedList : IEnumerable
    {
        /// <summary>
        /// 当前页索引
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        int TotalRecordCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        int TotalPageCount { get; }
    }
    public interface IPagedList<T> : IEnumerable<T>, IPagedList { }
}
