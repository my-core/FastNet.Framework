using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.RabbitMQ
{
    /// <summary>
    /// RM消息传输载体
    /// </summary>
    public class MqMessagePayload
    {
        public MqMessagePayload()
        {
        }

        public string MessageID { get; set; }
        public string MessageType { get; set; }
        public DateTime CreatedTime { get; set; }
        public byte[] Message { get; set; }
    }

    /// <summary>
    /// 消息容器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MqMessage<T>
    {
        public MqMessage()
        {
            this.MessageID = Guid.NewGuid().ToString();
            this.CreatedTime = DateTime.Now;
            this.MessageType = typeof(T).Name;
        }

        public string MessageID { get; set; }
        public string MessageType { get; set; }
        public DateTime CreatedTime { get; set; }
        public T Message { get; set; }

        public MqMessagePayload ToMessagePayload()
        {
            return new MqMessagePayload
            {
                MessageID = MessageID,
                MessageType = MessageType,
                CreatedTime = CreatedTime,
                Message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Message))
            };
        }
    
    }
}
