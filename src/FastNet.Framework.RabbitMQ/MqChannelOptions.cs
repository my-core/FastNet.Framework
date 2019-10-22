using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNet.Framework.RabbitMQ
{
    public class MqChannelOptions
    {
        public MqChannelOptions()
        { }
        /// <summary>
        /// 客户端服务名
        /// </summary>
        public string ClientServiceName { get; set; }
        /// <summary>
        /// 主机名
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 虚拟主机，一个broker里可以开设多个vhost，用作不同用户的权限分离
        /// </summary>
        public string VirtualHost { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 交换机名称，指定消息按什么规则(RoutingKey)，路由到哪个队列(Queue)
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// 交换机类型(Fanout-广播、Direct-精准 和 Topic-通配)
        /// </summary>
        public string ExchangeType { get; set; }
        /// <summary>
        /// 交换机持久化
        /// </summary>
        public bool ExchangeDurable { get; set; } = false;
        /// <summary>
        /// 自动删除
        /// </summary>
        public bool ExchangeAutoDelete { get; set; } = false;
        /// <summary>
        /// 拓展参数
        /// </summary>
        public Dictionary<string, object> ExchangeArguments { get; set; }
        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// 队列持久化
        /// </summary>
        public bool QueueDurable { get; set; } = false;
        /// <summary>
        /// 排他队列，如果一个队列被声明为排他队列，该队列仅对首次声明它的连接可见，并在连接断开时自动删除
        /// </summary>
        public bool QueueExclusive { get; set; } = false;
        /// <summary>
        /// 自动删除，如果该队列没有任何订阅的消费者的话，该队列会被自动删除。这种队列适用于临时队列
        /// </summary>
        public bool QueueAutoDelete { get; set; } = false;
        /// <summary>
        /// 拓展参数
        /// </summary>
        public Dictionary<string, object> QueueArguments { get; set; }
        /// <summary>
        /// 消息是否需要确认
        /// </summary>
        public bool QueueNoAck { get; set; } = true;
        /// <summary>
        /// 路由器
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// 获取绑定队列的RoutingKey
        /// </summary>
        /// <returns></returns>
        public List<string> GetQueueBindRoutingKey()
        {
            List<string> queueBindRoutingKeys = new List<string>();
            if (!string.IsNullOrWhiteSpace(RoutingKey))
            {
                queueBindRoutingKeys = RoutingKey.Split(new char[] { ';', '|', ',' }).Select(k => k.Trim()).ToList();
            }
            return queueBindRoutingKeys;
        }
    }
}
