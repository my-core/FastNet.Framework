using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.NetCrawler
{
    /// <summary>
    /// 爬虫请求成功后的回调函数
    /// </summary>
    /// <param name="callbackEventArgs"></param>
    public delegate void CallbackHandler(CallbackEventArgs callbackEventArgs);
    /// <summary>
    /// 爬虫管理器：负责创建多线程爬虫、创建爬虫请求
    /// </summary>
    public class CrawlerManager
    {
        /// <summary>
        /// 抓取请求队列
        /// </summary>
        //public PriorityQueue<CrawlRequest> crawlRequests = new PriorityQueue<CrawlRequest>(new PairCompare());
        public Stack<CrawlRequest> crawlRequests = new Stack<CrawlRequest>();
        /// <summary>
        /// 每次执行的线程数
        /// </summary>
        private int _threadCount = 10;
        /// <summary>
        /// 线程组
        /// </summary>
        public CrawlThread[] crawlThreads;
        /// <summary>
        /// 构造函数(指定线程数)
        /// </summary>
        /// <param name="threadCount">线程数</param>
        public CrawlerManager(int threadCount)
        {
            _threadCount = threadCount;
        }

        /// <summary>
        /// 创建抓取请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        public void Request(string url, CallbackHandler callBack,Dictionary<string,object> metadata=null,int depth=0)
        {
            CrawlRequest request = new CrawlRequest { Url = url, Metadata = metadata, Depth = depth };
            request.Callback += callBack;
            crawlRequests.Push(request);
        }

        /// <summary>
        /// 初始化线程
        /// </summary>
        public void InitCrawThread()
        {
            //创建线程
            crawlThreads = new CrawlThread[_threadCount];
            for (int i = 0; i < _threadCount; i++)
            {
                CrawlThread crawlThread = new CrawlThread(this);
                crawlThread.Name = i.ToString();
                crawlThread.Start();
                crawlThreads[i] = crawlThread;
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < crawlThreads.Length; i++)
            {
                crawlThreads[i].Abort();
            }
        }
    }
}
