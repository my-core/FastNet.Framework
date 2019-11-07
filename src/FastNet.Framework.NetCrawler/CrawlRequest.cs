using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.NetCrawler
{
    /// <summary>
    /// 爬虫请求
    /// </summary>
    public class CrawlRequest
    {
        /// <summary>
        /// 爬取的url地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 扩展数据，该参数将传到Callback函数
        /// </summary>
        public Dictionary<string,object> Metadata { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CallbackHandler Callback;
        /// <summary>
        /// 深度
        /// </summary>
        public int Depth = 0;
    }
}
