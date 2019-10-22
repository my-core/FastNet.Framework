using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.RabbitMq.Publisher
{
    public class PublisherService : IHostedService
    {
        private ILogger<PublisherService> _logger;
        private TestPublisher _testPublisher;
        private Timer _timer;
        public PublisherService(ILogger<PublisherService> logger, TestPublisher testPublisher)
        {
            _logger = logger;
            _testPublisher = testPublisher;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("ConsumerService start");
            _testPublisher.Initialize();
            _logger.LogDebug("ConsumerService start successed");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _testPublisher.RaiseMessage(new TestNotification { UserID = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _testPublisher.Dispose();
            return Task.CompletedTask;
        }
    }
}
