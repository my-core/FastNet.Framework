using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.Redis
{
    /// <summary>
    /// Redis配置
    /// </summary>
    public class RedisOptions
    {
        //redis服务地址
        public string EndPoint { get; set; }
        public string Password { get; set; }
        public int DatabaseNo { get; set; }
    }
}
