using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.RabbitMq.Consumer
{
    public class ConsumerService : IHostedService
    {
        private ILogger<ConsumerService> _logger;
        private TestConsumer _testConsumer;
        public ConsumerService(ILogger<ConsumerService> logger, TestConsumer testConsumer)
        {
            _logger = logger;
            _testConsumer = testConsumer;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("ConsumerService start");
            _testConsumer.Initialize();
            _logger.LogDebug("ConsumerService start successed");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _testConsumer.Dispose();
            return Task.CompletedTask;
        }
    }
}
