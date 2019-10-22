
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNet.Framework.RabbitMQ
{
    public class MqChannel
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private IConnection _connection;
        protected IModel _channel;
        private MqChannelOptions _options;
        // In general, IModel instances should not be used by more than one thread simultaneously:
        // application code should maintain a clear notion of thread ownership for IModel instances.
        // If more than one thread needs to access a particular IModel instances, 
        // the application should enforce mutual exclusion itself.
        protected readonly object _pipelining = new object();

        public MqChannel( MqChannelOptions options)
        {
            _options = options ?? throw new ArgumentNullException("MqChannelOptions");
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        public void Connect()
        {
            lock (_pipelining)
            {
                try
                {
                    _logger.Debug("Connect, begin to connect to mq.");
                    var connectionFactory = BuildConnectionFactory();
                    _connection = connectionFactory.CreateConnection();
                    _connection.CallbackException += OnConnectionCallbackException;
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    BindChannel();
                    _logger.Debug("Connect, connect to mq successfully.");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Connect, connect to mq failed due to [{0}].", ex.Message);
                    throw;
                }
            }
        }
        
        /// <summary>
        /// 主动断开连接
        /// </summary>
        public void Disconnect()
        {
            _logger.Debug("Disconnect, begin to disconnect to mq.");
            lock (_pipelining)
            {
                try
                {
                    if (_channel != null)
                    {
                        _channel.CallbackException -= OnChannelCallbackException;
                        _channel.ModelShutdown -= OnModelShutdown;
                        _channel.Close();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Disconnect, disconnect to mq channel failed due to [{0}].", ex.Message);
                }
                finally
                {
                    _channel = null;
                }

                try
                {
                    if (_connection != null)
                    {
                        _connection.CallbackException -= OnConnectionCallbackException;
                        _connection.ConnectionShutdown -= OnConnectionShutdown;
                        _connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Disconnect, disconnect to mq failed due to [{0}].", ex.Message);
                }
                finally
                {
                    _connection = null;
                }
            }
            _logger.Debug("Disconnect, disconnect to mq successfully.");
        }

        /// <summary>
        /// 异常断开连接
        /// </summary>
        protected void AbnormalDisconnect()
        {
            lock (_pipelining)
            {
                try
                {
                    if (_channel != null)
                    {
                        _channel.CallbackException -= OnChannelCallbackException;
                        _channel.ModelShutdown -= OnModelShutdown;
                        _channel.Close();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "AbnormalDisconnect, disconnect to mq channel failed due to [{0}].", ex.Message);
                }
                finally
                {
                    _channel = null;
                }

                try
                {
                    if (_connection != null)
                    {
                        _connection.CallbackException -= OnConnectionCallbackException;
                        _connection.ConnectionShutdown -= OnConnectionShutdown;
                        _connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "AbnormalDisconnect, disconnect to mq connection failed due to [{0}].", ex.Message);
                }
                finally
                {
                    _connection = null;
                }
            }
            _logger.Warn("AbnormalDisconnect, abnormal disconnect to mq.");
        }

        /// <summary>
        /// 创建ConnectionFactory
        /// </summary>
        /// <returns></returns>
        private ConnectionFactory BuildConnectionFactory()
        {
            var factory = new ConnectionFactory()
            {
                Protocol = Protocols.DefaultProtocol,
                HostName = _options.HostName,
                Port = _options.Port,
                VirtualHost = _options.VirtualHost,
                UserName = _options.UserName,
                Password = _options.Password,
                RequestedConnectionTimeout = 60000,
                RequestedHeartbeat = 30,
            };
            factory.ClientProperties.Add("Application Name", _options.ClientServiceName);
            factory.ClientProperties.Add("Application Connected Time", DateTime.Now.ToString("o"));           
            return factory;
        }

        /// <summary>
        /// 绑定信道
        /// </summary>
        private void BindChannel()
        {
            // Brokers provide four exchange types: Direct, Fanout, Topic and Headers.
            // Durability (exchanges survive broker restart)
            // Auto-delete (exchange is deleted when all queues have finished using it)
            // Arguments (these are broker-dependent)
            string exchangeType = _options.ExchangeType;
            bool exchangeDurable = _options.ExchangeDurable;
            bool exchangeAutoDelete = _options.ExchangeAutoDelete;
            var exchangeArguments = _options.ExchangeArguments;

            // Durable (the queue will survive a broker restart)
            // Exclusive (used by only one connection and the queue will be deleted when that connection closes)
            // Auto-delete (queue is deleted when last consumer unsubscribed)
            // Arguments (some brokers use it to implement additional features like message TTL)
            bool queueDurable = _options.QueueDurable;
            bool queueExclusive = _options.QueueExclusive;
            bool queueAutoDelete = _options.QueueAutoDelete;
            var queueArguments = _options.QueueArguments;

            _channel = _connection.CreateModel();
            _channel.CallbackException += OnChannelCallbackException;
            _channel.ModelShutdown += OnModelShutdown;

            if (!string.IsNullOrEmpty(_options.ExchangeName))
            {
                //声明交换机
                _channel.ExchangeDeclare(_options.ExchangeName, exchangeType, exchangeDurable, exchangeAutoDelete, exchangeArguments);
            }

            if (!string.IsNullOrEmpty(_options.QueueName))
            {
                //声明队列
                var queueStatus = _channel.QueueDeclare(_options.QueueName, queueDurable, queueExclusive, queueAutoDelete, queueArguments);
                List<string> queueBindRoutingKeys = new List<string>();
                if (!string.IsNullOrWhiteSpace(_options.RoutingKey))
                {
                    queueBindRoutingKeys = _options.RoutingKey.Split(new char[] { ';', '|' }).Select(k => k.Trim()).ToList();
                }
                if (queueBindRoutingKeys.Count == 0)
                {
                    _channel.QueueBind(_options.QueueName, _options.ExchangeName, string.Empty);
                }
                else
                {
                    foreach (var routingKey in queueBindRoutingKeys)
                    {
                        _channel.QueueBind(_options.QueueName, _options.ExchangeName, routingKey);
                    }
                }
                _channel.BasicQos(0, 1, false);
                _logger.Debug("BindChannel, QueueName[{0}], ConsumerCount[{1}], MessageCount[{2}].",
                    queueStatus.QueueName, queueStatus.ConsumerCount, queueStatus.MessageCount);
            }
        }

        #region Connection Event
        /// <summary>
        /// 连接关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.Error("OnConnectionShutdown, connection is shutdown due to [{0}].",
                e == null ? "" :
                    string.Format("ClassId[{0}], MethodId[{1}], ReplyCode[{2}], ReplyText[{3}], Cause[{4}]",
                        e.ClassId, e.MethodId, e.ReplyCode, e.ReplyText, e.Cause));
            AbnormalDisconnect();
        }
        /// <summary>
        /// 连接异常回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectionCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            _logger.Error("OnConnectionCallbackException, [{0}].",
                e == null ? "" :
                    string.Format("Exception[{0}], Detail[{1}]",
                        e.Exception, e.Detail == null ? "" : string.Join(",", e.Detail.Values)));
        }

        /// <summary>
        /// 信道关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModelShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.Error("OnChannelShutdown, channel is shutdown due to [{0}].",
                e == null ? "" :
                    string.Format("ClassId[{0}], MethodId[{1}], ReplyCode[{2}], ReplyText[{3}], Cause[{4}]",
                        e.ClassId, e.MethodId, e.ReplyCode, e.ReplyText, e.Cause));
            AbnormalDisconnect();
        }
        /// <summary>
        /// 信道异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChannelCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            _logger.Error("OnChannelCallbackException, [{0}].",
                e == null ? "" :
                    string.Format("Exception[{0}], Detail[{1}]",
                        e.Exception, e.Detail == null ? "" : string.Join(",", e.Detail.Values)));
        }
        #endregion
    }
}
