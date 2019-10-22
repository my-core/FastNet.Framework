using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastNet.Framework.Dapper
{
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        /// <summary>
        /// 当前页索引
        /// </summary>
        public int PageIndex { get; set; }        
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalRecordCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPageCount { get; }

        /// <summary>
        /// 构造分页数据
        /// </summary>
        /// <param name="currentPageItems"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalItemCount"></param>
        public PagedList(IEnumerable<T> currentPageItems, int pageIndex, int pageSize, int totalItemCount)
        {
            AddRange(currentPageItems);
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalRecordCount = totalItemCount;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)PageSize);
        }
    }
}
