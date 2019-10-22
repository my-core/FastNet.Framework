
using NLog;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.RabbitMQ
{
    class MqPublisherChannel : MqChannel
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private MqChannelOptions _options;
        public MqPublisherChannel(MqChannelOptions options) : base(options)
        {
            _options = options;
        }

        public void Publish(byte[] message)
        {
            Publish(message, string.Empty);
        }

        public void Publish(byte[] message, string routingKey)
        {
            // This flag tells the server how to react if the message cannot be routed to a queue.
            // If this flag is set, the server will return an unroutable message with a Return method. 
            // If this flag is zero, the server silently drops the message.
            bool mandatory = false;
            Publish(message, routingKey, mandatory);
        }

        public void Publish(byte[] message, string routingKey, bool mandatory)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (routingKey == null)
                throw new ArgumentNullException("routingKey");

            lock (_pipelining)
            {
                try
                {
                    _channel.BasicPublish(_options.ExchangeName, routingKey, mandatory, null, message);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    AbnormalDisconnect();
                    throw;
                }
            }
        }

        public void Publish(byte[] message, string routingKey, bool mandatory, IDictionary<string, object> headers)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (routingKey == null)
                throw new ArgumentNullException("routingKey");
            if (headers == null)
                throw new ArgumentNullException("headers");

            lock (_pipelining)
            {
                try
                {
                    var basicProperties = _channel.CreateBasicProperties();
                    if (basicProperties.Headers == null)
                    {
                        basicProperties.Headers = headers;
                    }
                    else
                    {
                        foreach (var item in headers)
                        {
                            basicProperties.Headers[item.Key] = item.Value;
                        }
                    }
                    _channel.BasicPublish(_options.ExchangeName, routingKey, mandatory, basicProperties, message);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    AbnormalDisconnect();
                    throw;
                }
            }
        } 
    }
}
