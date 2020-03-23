using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.CSRedis
{
    /// <summary>
    /// Redis配置基类
    /// </summary>
    public class RedisOptions
    {
        public string ConnectionString { get; set; }

        public string MasterName { get; set; }
        /// <summary>
        /// 1-哨兵
        /// </summary>
        public int IsSentinelModel { get; set; }
        /// <summary>
        /// 哨兵节点
        /// </summary>
        public string[] Sentinels { get; set; }

    }
}
