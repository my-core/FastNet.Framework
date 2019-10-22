
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.RabbitMQ
{
    public class MqPublisher : IDisposable
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private MqChannelOptions _options;
        private MqPublisherChannel _channel;
        public MqPublisher(MqChannelOptions options)
        {
            _options = options;
            _channel = new MqPublisherChannel(options);
        }

        /// <summary>
        /// 初始化Publisher
        /// </summary>
        public void Initialize()
        {
            try
            {
                _channel.Connect();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occured when Initialize mq");
            }
            _logger.Debug($"Init successed, QueueName[{_options.QueueName}], RouteKey[{_options.RoutingKey}]");
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            _channel.Disconnect();
        }

        /// <summary>
        /// 发布消息（延迟）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="delaySeconds"></param>
        public void RaiseMessage<T>(MqMessage<T> message, int delaySeconds)
        {
            _logger.Debug($"RaiseMessage, message[{JsonConvert.SerializeObject(message)}],delay [{delaySeconds}] seconds");

            MqMessagePayload messagePayload = message.ToMessagePayload();
            byte[] messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messagePayload));

            var headers = new Dictionary<string, object> { { "x-delay", delaySeconds * 1000 } };
            List<string> routingKeys = _options.GetQueueBindRoutingKey();
            routingKeys.ForEach(key =>
            {
                _logger.Debug($"Publish message to routeKey[{key}], and delay [{delaySeconds}] seconds.");
                _channel.Publish(messageBody, key, false, headers);
            });
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public void RaiseMessage<T>(MqMessage<T> message)
        {
            _logger.Debug($"RaiseMessage, message[{JsonConvert.SerializeObject(message)}]");

            MqMessagePayload messagePayload = message.ToMessagePayload();
            byte[] messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messagePayload));

            List<string> routingKeys = _options.GetQueueBindRoutingKey();
            routingKeys.ForEach(key =>
            {
                _logger.Debug($"Publish message to routeKey[{key}].");
                _channel.Publish(messageBody, key, false);
            });
        }
    }
}
