using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Yousei
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddLog4Net();
                })
                .ConfigureServices(services =>
                {
                    ModuleRegistry.RegisterModules(services);
                    services.AddSingleton<JobRegistry>();
                    services.AddSingleton<JobFlowCreator>();
                    services.AddHostedService<JobService>();
                });
    }
}
