using HtmlAgilityPack;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FastNet.Framework.NetCrawler
{
    /// <summary>
    /// 爬虫线程
    /// </summary>
    public class CrawlThread
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 线程
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// 每一个线程都包含Crawler主体
        /// </summary>
        private CrawlerManager _crawler { get; set; }

        /// <summary>
        /// 线程名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 当前爬行URl
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 构造函数（为线程的采集主体赋值）
        /// </summary>
        /// <param name="Linker"></param>
        public CrawlThread(CrawlerManager crawler)
        {
            _thread = new Thread(DoWork);
            _thread.IsBackground = true;
            _crawler = crawler;
        }

        /// <summary>
        /// 开始线程
        /// </summary>
        /// <param name="data"></param>
        public void DoWork(object data)
        {
            try
            {
                while (true)
                {
                    if (_crawler.crawlRequests.Count == 0)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                        continue;
                    }
                    CrawlRequest crawlRequest = null;
                    lock (_crawler.crawlRequests)
                    {                        
                        crawlRequest = _crawler.crawlRequests.Pop();
                    }
                    if (crawlRequest == null)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                        continue;
                    }
                    try
                    {   
                        _logger.Debug($"begin request url[{crawlRequest.Url}]");
                        // 获取页面
                        HtmlWeb web = new HtmlWeb();
                        web.OverrideEncoding = Encoding.GetEncoding("gb2312");
                        var htmlDoc = web.Load(crawlRequest.Url);
                        crawlRequest.Callback(new CallbackEventArgs(crawlRequest.Url, htmlDoc, crawlRequest.Metadata, Name));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"request url[{crawlRequest.Url}]");
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"DoWork failed");
                // 线程被放弃
            }
        }

        /// <summary>
        /// 线程开始
        /// </summary>
        public void Start()
        {
            _thread.Start(this);
        }
        /// <summary>
        /// 线程中止
        /// </summary>
        public void Abort()
        {
            _thread.Abort();
        }

    }
}
