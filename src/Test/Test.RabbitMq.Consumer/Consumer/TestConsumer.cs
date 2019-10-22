using FastNet.Framework.RabbitMQ;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.RabbitMq.Consumer
{
    public class TestConsumer : MqConsumer
    {
        public TestConsumer(IOptionsFactory<MqChannelOptions> options, ITestHandler testHandler)
            : base(options.Create(typeof(TestConsumer).Name), testHandler)
        {
            var o = options.Create(this.GetType().Name);
        }
    }
}
