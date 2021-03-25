using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yousei.Shared;
using Yousei.Internal;
using YouseiReloaded.Internal;
using YouseiReloaded.Internal.Connectors.Internal;
using YouseiReloaded.Serialization.Json;
using YouseiReloaded.Serialization.Yaml;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.Builder;

namespace Yousei
{

    internal class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigurationProvider, YamlConfigurationProvider>();
            services.AddSingleton<IConfigurationDatabase, ConfigurationProviderDatabase>();
            services.AddSingleton<IConnectorRegistry, ConnectorRegistry>();
            services.AddSingleton<IFlowActor, FlowActor>();
            services.AddSingleton<EventHub>();
            services.AddSingleton<FlowManager>();
            services.AddSingleton<InternalConnector>();
            services.AddHostedService<MainService>();
        }
    }
}