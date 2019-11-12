
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.Consul
{
    /// <summary>
    /// Consul扩展
    /// </summary>
    public static class ConsulExtensions
    {
        /// <summary>
        /// 注册consul服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            var consulOptions = configuration.GetSection("JwtOptions").Get<ConsulOptions>();
            AddConsul(services, consulOptions);
        }

        public static void AddConsul(this IServiceCollection services, ConsulOptions consulOptions)
        {
            //注册consul配置
            services.AddSingleton(consulOptions);
            //注册consul同步服务
            services.AddHostedService<ConsulHostedService>();
            //httpclient
            services.AddTransient<ConsulHttpClient>();
        }
    }
}
