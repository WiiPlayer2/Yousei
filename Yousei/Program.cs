using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yousei.Shared;
using Yousei.Internal;
using Yousei.Internal.Connectors.Internal;
using Yousei.Serialization.Json;
using Yousei.Serialization.Yaml;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;

namespace Yousei
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new[]
                {
                    new ParameterConverter(),
                }
            };

            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static void ConfigureLogging(ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddLog4Net();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureLogging(ConfigureLogging)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}