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
using Yousei.Api;
using Yousei.Api.Extensions;
using Newtonsoft.Json.Linq;
using HotChocolate.Utilities;
using HotChocolate.Types;
using Yousei.Api.SchemaType;

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

            app.UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapGraphQL();
                });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Internal
            services.AddSingleton<IConfigurationProvider, YamlConfigurationProvider>();
            services.AddSingleton<IConfigurationDatabase, ConfigurationProviderDatabase>();
            services.AddSingleton<IConnectorRegistry, ConnectorRegistry>();
            services.AddSingleton<IFlowActor, FlowActor>();
            services.AddSingleton<EventHub>();
            services.AddSingleton<FlowManager>();
            services.AddSingleton<InternalConnector>();
            services.AddHostedService<MainService>();

            // Api
            services.AddSingleton<IApi, InternalApi>();
            services.AddGraphQLServer()
                .BindRuntimeType<JToken, JsonType>()
                .AddType<Query>()
                .AddType<ConfigurationExtension>()
                .ModifyRequestOptions(options =>
                {
                    options.IncludeExceptionDetails = true;
                });
        }
    }
}