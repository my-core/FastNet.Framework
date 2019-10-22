using FastNet.Framework.RabbitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.RabbitMq.Publisher
{
    public class TestPublisher
    {
        private MqPublisher _mqPublisher;
        private ILogger<TestPublisher> _log;

        public TestPublisher(ILogger<TestPublisher> log, IOptionsFactory<MqChannelOptions> options)
        {
            _log = log;
            _mqPublisher = new MqPublisher(options.Create(this.GetType().Name));
            
        }

        public void Initialize()
        {
            _mqPublisher.Initialize();
        }

        public void RaiseMessage<T>(T messsage)
        {
            _log.LogDebug($"RaiseMessage->MessageType[{typeof(T).Name}],{JsonConvert.SerializeObject(messsage)}");
            _mqPublisher.RaiseMessage(new MqMessage<T> { Message = messsage });
        }

        public void Dispose()
        {
            _mqPublisher.Dispose();

        }
    }
}
