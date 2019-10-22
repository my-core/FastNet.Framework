using FastNet.Framework.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace Test.RabbitMq.Consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new HostBuilder()
               .ConfigureAppConfiguration((hostContext, configBuilder) =>
               {
                   var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                   configBuilder.SetBasePath(Directory.GetCurrentDirectory());
               })
               .ConfigureServices((hostContext, services) =>
               {
                   var loggerFactory = new LoggerFactory();
                   loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
                   NLog.LogManager.LoadConfiguration("nlog.config");


                   services.Configure<MqChannelOptions>(typeof(TestConsumer).Name,
                       o =>
                       {
                           o.ClientServiceName = "Test.RabbitMq.Consumer";
                           o.HostName = "127.0.0.1";
                           o.Port = 5672;
                           o.VirtualHost = "/";
                           o.UserName = "guest";
                           o.Password = "guest";
                           o.ExchangeName = "fastnet.framework";
                           o.ExchangeType = "direct";
                           o.QueueName = "fastnet.framework.test";
                           o.RoutingKey = "fastnet.framework.test";
                       });
                   services.AddSingleton<ITestHandler, TestHandler>();
                   services.AddSingleton<TestConsumer>();
                   services.AddHostedService<ConsumerService>();
               })
               .RunConsoleAsync();

            Console.WriteLine("Application Started");
            var ended = new ManualResetEventSlim();
            var starting = new ManualResetEventSlim();

            AssemblyLoadContext.Default.Unloading += ctx =>
            {
                starting.Set();
                ended.Wait();
            };
            starting.Wait();
            Thread.Sleep(5000);
            ended.Set();
            NLog.LogManager.Shutdown();
            Console.WriteLine("Application Shutdown");

        }
    }
}
