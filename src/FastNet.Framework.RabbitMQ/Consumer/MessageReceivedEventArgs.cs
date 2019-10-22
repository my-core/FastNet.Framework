using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.RabbitMQ
{
    public class MessageReceivedEventArgs
    {
        /// <summary>
        /// 消费者标识
        /// </summary>
        public string ConsumerTag { get; set; }
        /// <summary>
        /// 标识信道中投递的消息唯一标识
        /// </summary>
        public ulong DeliveryTag { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
        public byte[] Body { get; set; }
    }
}
