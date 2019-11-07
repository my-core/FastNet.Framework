using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.NetCrawler
{
    /// <summary>
    /// 爬虫实例：针对不同规则可以创建多个爬虫实例，继承此类即可扩展
    /// </summary>
    public abstract class Crawler : IDisposable
    {
        public CrawlerManager _crawler;
        /// <summary>
        /// 默认参数
        /// </summary>
        public Crawler()
        {
            _crawler = new CrawlerManager(10);
        }
        /// <summary>
        /// 指定参数构造
        /// </summary>
        /// <param name="crawlOptions"></param>
        public Crawler(CrawlOptions crawlOptions)
        {
            _crawler = new CrawlerManager(crawlOptions.MaxCrawlThread);
            _crawler.InitCrawThread();
        }
        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public virtual void Start(string url, CallbackHandler callback)
        {
            Request(url, callback);
        }
        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <param name="metadata"></param>
        /// <param name="depth"></param>
        public void Request(string url, CallbackHandler callback, Dictionary<string, object> metadata = null, int depth = 0)
        {
            _crawler.Request(url, callback, metadata, depth);
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _crawler.Dispose();
        }
    }
}
