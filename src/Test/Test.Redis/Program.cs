using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using FastNet.Framework.Redis;

namespace Test.Redis
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().RunAsync().GetAwaiter().GetResult();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    Console.WriteLine($"Host Environment:{environment}");
                    configBuilder.SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                       //.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                       .AddEnvironmentVariables();

                })
                .ConfigureServices((context, services) =>
                {
                    NLog.LogManager.LoadConfiguration("nlog.config");
                    services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                        //redis
                        services.Configure<RedisOptions>(typeof(TestRedisManager).Name, o => context.Configuration.GetSection("TestRedisOptions").Bind(o));
                    services.AddSingleton<ITestRedisManager, TestRedisManager>();

                    //test
                    var redis = services.BuildServiceProvider().GetService<ITestRedisManager>();
                    //redis.SetStringKey("userid", "123456789");
                    //Console.WriteLine($"{redis.GetStringKey("userid")}");
                    //redis.RemoveStringKey("userid");

                    //UserInfo userInfo = new UserInfo() { UserID = 1, UserName = "张三" };
                    //redis.SetUserInfo("users", userInfo.UserID.ToString(), userInfo);

                    List<UserInfo> userList = new List<UserInfo>();
                    for(int i=0; i < 10; i++)
                    {
                        userList.Add(new UserInfo { UserID = i, UserName = $"张三{i}" });
                    }
                    redis.SetUserList("users", userList);



                })
                .ConfigureLogging((logging, builder) =>
                {
                    builder.AddNLog();
                })
                .UseConsoleLifetime();
    }
}

