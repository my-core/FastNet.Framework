using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.NetCrawler
{
    /// <summary>
    /// 爬虫配置，目前只设置最大线程数
    /// </summary>
    public class CrawlOptions
    {
        public int MaxCrawlThread { get; set; }
    }
}
