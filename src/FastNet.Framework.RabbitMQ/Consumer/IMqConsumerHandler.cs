using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.RabbitMQ
{
    public interface IMqConsumerHandler
    {
        void OnMessageReceived(MqMessagePayload message);
    }
}
