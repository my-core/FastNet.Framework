using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastNet.Framework.Consul
{
    /// <summary>
    /// 用于同步注册中心服务
    /// </summary>
    public class ConsulHostedService : IHostedService
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// oss 客户端实例
        /// </summary>
        private ConsulClient _consulClient;
        /// <summary>
        /// Consul配置
        /// </summary>
        private readonly ConsulOptions _consulConfig;
        /// <summary>
        /// ApplicationLifetime
        /// </summary>
        private readonly IApplicationLifetime _lifetime;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public ConsulHostedService(ILogger<ConsulHostedService> logger, IApplicationLifetime lifetime, ConsulOptions consulOptions)
        {
            _logger = logger;
            _lifetime = lifetime;
            _consulConfig = consulOptions;
            //应用服务ip，未配置则默认系统自动获取应用所在机器地址
            if (string.IsNullOrEmpty(_consulConfig.ServiceAddress))
                _consulConfig.ServiceAddress = GetAddress();

        }
        /// <summary>
        /// start service
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterConsul();
            SyncProcess();
            return Task.CompletedTask;
        }

        /// <summary>
        /// stop service
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger?.LogInformation($"ConsulService.StopAsync");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 注册Consul Service
        /// </summary>
        /// <returns></returns>
        private void RegisterConsul()
        {
            try
            {
                //请求注册的 Consul 地址
                var url = new Uri($"http://{_consulConfig.ConsulIP}:{_consulConfig.ConsulPort}");
                _consulClient = new ConsulClient(config => config.Address = url);

                //判断是否注册服务
                if (!_consulConfig.IsRegisterSelf)
                    return;

                //健康检查
                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(_consulConfig.HealthCheckInterval),//健康检查时间间隔，或者称为心跳间隔
                    HTTP = $"http://{_consulConfig.ServiceAddress}:{_consulConfig.ServicePort}/{_consulConfig.HealthCheckApi.TrimStart('/')}",//健康检查地址
                    Timeout = TimeSpan.FromSeconds(5)
                };
                // Register service with consul
                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    ID = Guid.NewGuid().ToString(),
                    Name = _consulConfig.ServiceName,
                    Address = _consulConfig.ServiceAddress,
                    Port = _consulConfig.ServicePort,
                    //添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别
                    Tags = new[] { $"urlprefix-/{_consulConfig.ServiceName}" }
                };
                //服务启动时注册，内部实现其实就是使用 Consul API 进行注册（HttpClient发起）
                _consulClient.Agent.ServiceRegister(registration).Wait();
                _lifetime.ApplicationStopping.Register(() =>
                {
                    _consulClient.Agent.ServiceDeregister(registration.ID).Wait();//服务停止时取消注册 
                });
                _logger?.LogInformation($"ConsulService.RegisterConsul->Success（ID:{registration.ID})");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "ConsulService.RegisterConsul->Failed");
            }
        }

        /// <summary>
        /// 同步服务信息线程
        /// </summary>
        private void SyncProcess()
        {
            if (_consulConfig.SyncServiceNames == null || _consulConfig.SyncServiceNames.Length == 0)
                return;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    //同步服务信息
                    SyncServices();
                    //同步间隔
                    Thread.Sleep(TimeSpan.FromSeconds(_consulConfig.SyncServiceInterval));
                }
            });
        }

        /// <summary>
        /// 同步服务信息
        /// </summary>
        private void SyncServices()
        {
            if (_consulConfig.SyncServiceNames == null || _consulConfig.SyncServiceNames.Length == 0)
                return;
            try
            {
                foreach (var serviceName in _consulConfig.SyncServiceNames)
                {
                    var result = _consulClient.Health.Service(serviceName).Result;
                    ConsulCache.Instance.SetServiceInfo(serviceName, result.Response);
                    _logger?.LogInformation("ConsulService.SyncServices->Success:" + JsonConvert.SerializeObject(result.Response));
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "ConsulService.SyncServices->Failed");
            }
        }

        /// <summary>
        /// 获取机器地址
        /// </summary>
        /// <returns></returns>
        private string GetAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;
                var properties = network.GetIPProperties();
                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (IPAddress.IsLoopback(address.Address))
                        continue;
                    return address.Address.ToString();
                }
            }
            return string.Empty;
        }
    }
}
