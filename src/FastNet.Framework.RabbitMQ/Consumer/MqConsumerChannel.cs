
using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.RabbitMQ
{
    class MqConsumerChannel : MqChannel
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private MqChannelOptions _options;
        private EventingBasicConsumer _consumer = null;
        private string _consumerTag = string.Empty;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public MqConsumerChannel(MqChannelOptions options) : base(options)
        {
            _options = options;
        }

        public void StartConsume()
        {
            lock (_pipelining)
            {
                if (_consumer != null) return;

                _consumer = new EventingBasicConsumer(_channel);
                _consumer.Registered += OnRegistered;
                _consumer.Unregistered += OnUnregistered;
                _consumer.Received += OnReceived;
                _consumer.Shutdown += OnShutdown;
                _consumerTag = _channel.BasicConsume(_options.QueueName, _options.QueueNoAck, _consumer);

                _logger.Debug("StartConsume, start to consumer [{0}] on tag [{1}] with options [{2}].",
                    _options.QueueName, _consumerTag, JsonConvert.SerializeObject(_options));
            }
        }

        #region Consumer Event
        private void OnReceived(object sender, BasicDeliverEventArgs e)
        {
            if (MessageReceived != null)
            {
                var message = new MessageReceivedEventArgs()
                {
                    ConsumerTag = e.ConsumerTag,
                    DeliveryTag = e.DeliveryTag,
                    ExchangeName = e.Exchange,
                    RoutingKey = e.RoutingKey,
                    Body = e.Body,
                };
                MessageReceived(this, message);
            }
            else
            {
                _logger.Debug("OnReceived, message discarded due to handler not found, " +
                    "ConsumerTag[{0}], DeliveryTag[{1}], Exchange[{2}], RoutingKey[{3}], BodyLength[{4}].",
                    e.ConsumerTag, e.DeliveryTag, e.Exchange, e.RoutingKey, e.Body == null ? "" : e.Body.Length.ToString());
            }
        }
        private void OnShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.Error("OnShutdown, consumer [{0}] on [{1}] shutdown due to [{2}].",
                _options.QueueName, _consumerTag, e);
        }
        private void OnUnregistered(object sender, ConsumerEventArgs e)
        {
            _logger.Debug("OnUnregistered, consumer [{0}] on tag [{1}] unregistered to consumer tag [{2}].",
                _options.QueueName, _consumerTag, e.ConsumerTag);
        }

        private void OnRegistered(object sender, ConsumerEventArgs e)
        {
            _logger.Debug("OnRegistered, consumer [{0}] on tag [{1}] registered to consumer tag [{2}].",
                _options.QueueName, _consumerTag, e.ConsumerTag);
        }
        #endregion

        /// <summary>
        /// 消息接收确认，只处理当前消息
        /// </summary>
        /// <param name="deliveryTag"></param>
        public void Ack(ulong deliveryTag)
        {
            Ack(deliveryTag, false);
        }

        /// <summary>
        /// 消息接收确认
        /// </summary>
        /// <param name="deliveryTag">标识信道中投递的消息</param>
        /// <param name="multiple">false表示只处理当前消息，true表示额外处理比deliveryTag小的消息</param>
        public void Ack(ulong deliveryTag, bool multiple)
        {
            if (_options.QueueNoAck)
                return;

            lock (_pipelining)
            {
                try
                {
                    _channel.BasicAck(deliveryTag, multiple);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Ack error occured");
                    AbnormalDisconnect();
                    throw;
                }
            }
        }

        /// <summary>
        /// 拒绝消息
        /// </summary>
        /// <param name="deliveryTag">标识信道中投递的消息</param>
        /// <param name="multiple">false表示只处理当前消息，true表示额外处理比deliveryTag小的消息</param>
        /// <param name="requeue">是否重回队列</param>
        public void Nack(ulong deliveryTag, bool multiple, bool requeue)
        {
            if (_options.QueueNoAck)
                return;

            lock (_pipelining)
            {
                try
                {
                    _channel.BasicNack(deliveryTag, multiple, requeue);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Nack error occured");
                    AbnormalDisconnect();
                    throw;
                }
            }
        }

        /// <summary>
        /// 取消确认 Reject
        /// </summary>
        /// <param name="deliveryTag"></param>
        /// <param name="requeue">是否重回队列</param>
        public void Reject(ulong deliveryTag, bool requeue)
        {
            if (_options.QueueNoAck)
                return;

            lock (_pipelining)
            {
                try
                {
                    _channel.BasicReject(deliveryTag, requeue);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Reject error occured");
                    AbnormalDisconnect();
                    throw;
                }
            }
        }

        /// <summary>
        /// Recover
        /// </summary>
        /// <param name="requeue"></param>
        public void Recover(bool requeue)
        {
            if (_options.QueueNoAck)
                return;

            lock (_pipelining)
            {
                try
                {
                    _channel.BasicRecover(requeue);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Recover error occured");
                    AbnormalDisconnect();
                    throw;
                }
            }
        }
    }
}
