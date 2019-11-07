using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.Redis
{
    /// <summary>
    /// Redis配置基类
    /// </summary>
    public class RedisOptions
    {
        public List<string> EndPoints { get; set; }
        public string Password { get; set; }
        public int DatabaseNo { get; set; }

    }
}
