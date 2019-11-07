using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.NetCrawler
{
    public class CallbackEventArgs
    {
        public CallbackEventArgs(string url, HtmlDocument htmlDocument, Dictionary<string, object> metadata,string threadName)
        {
            Url = url;
            HtmlDocument = htmlDocument;
            Metadata = metadata;
            ThreadName = threadName;
        }
        /// <summary>
        /// 爬虫URL地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 扩展数据
        /// </summary>
        public Dictionary<string,object> Metadata { get; set; }
        /// <summary>
        /// 抓取的Html文档
        /// </summary>
        public HtmlDocument HtmlDocument { get; set; }
        /// <summary>
        /// 线程名
        /// </summary>
        public string ThreadName { get; set; }
    }
}
