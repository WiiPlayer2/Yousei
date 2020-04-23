using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                .ConfigureServices(services =>
                {
                    services.AddLogging();
                    ModuleRegistry.RegisterModules(services);
                    services.AddSingleton<JobRegistry>();
                    services.AddSingleton<JobFlowCreator>();
                    services.AddHostedService<JobService>();
                });
    }
}
