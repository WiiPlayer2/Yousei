using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yousei.Shared;
using YouseiReloaded.Dummy;
using YouseiReloaded.Internal;
using YouseiReloaded.Serialization.Json;
using YouseiReloaded.Serialization.Yaml;

namespace YouseiReloaded
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new[]
                {
                    new ParameterConverter(),
                }
            };

            CreateHostBuilder(args).Build().Run();
        }

        private static void ConfigureLogging(ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddLog4Net();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigurationProvider, YamlConfigurationProvider>();
            services.AddSingleton<IConnectorRegistry, ConnectorRegistry>();
            services.AddSingleton<IFlowActor, FlowActor>();
            services.AddHostedService<MainService>();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureLogging(ConfigureLogging)
                .ConfigureServices(ConfigureServices);
    }
}