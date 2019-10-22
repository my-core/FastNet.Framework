
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.RabbitMQ
{
    public class MqConsumer : IDisposable
    {

        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private MqChannelOptions _options;
        private MqConsumerChannel _channel;
        private IMqConsumerHandler _consumerHandler;

        public MqConsumer(MqChannelOptions options, IMqConsumerHandler consumerHandler)
        {
            _options = options;
            _consumerHandler = consumerHandler;
        }

        /// <summary>
        /// 初始化Consumer
        /// </summary>
        public void Initialize()
        {
            _logger.Debug($"MqConsumerChannel Initialized start, options[{JsonConvert.SerializeObject(_options)}]");
            try
            {
                if (_channel == null)
                {
                    _channel = new MqConsumerChannel(_options);
                }
                _channel.Connect();
                _channel.MessageReceived += OnReceived;
                _channel.StartConsume();
                _logger.Debug($"MqConsumerChannel Initialized successed, QueueName[{_options.QueueName}], routekey[{_options.RoutingKey}]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"error occured when initialize mq");
            }
        }

        /// <summary>
        ///  接收消息处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<MqMessagePayload>(Encoding.UTF8.GetString(e.Body));
                _logger.Debug($"OnReceived, MessageType:[{message.MessageType}],ConsumerTag[{e.ConsumerTag}],ExchangeName[{e.ExchangeName}]");

                _consumerHandler.OnMessageReceived(message);
                _channel.Ack(e.DeliveryTag);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Received Error.");
                _channel.Nack(e.DeliveryTag, false, true);
            }
        } 

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            _channel.Disconnect();
        }
    }
}
