using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.RabbitMQ
{
    public abstract class MqConsumerHandler: IMqConsumerHandler
    {
        public MqConsumerHandler() { }
        
        public abstract void OnMessageReceived(MqMessagePayload message);
    }
}
