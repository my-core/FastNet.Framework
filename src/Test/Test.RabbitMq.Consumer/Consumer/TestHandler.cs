using FastNet.Framework.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.RabbitMq.Consumer
{
    public class TestHandler : ITestHandler
    {
        public void OnMessageReceived(MqMessagePayload message)
        {
            Console.WriteLine(Encoding.UTF8.GetString(message.Message));

        }
    }
}
