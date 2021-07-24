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
using Yousei.Api;
using Yousei.Api.Extensions;
using Newtonsoft.Json.Linq;
using HotChocolate.Utilities;
using HotChocolate.Types;
using Yousei.Api.SchemaType;
using Yousei.Api.Mutations;
using System.Reactive;
using Yousei.Internal.Database;
using Yousei.Api.Subscriptions;

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

            app.UseWebSockets()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapGraphQL();
                });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Internal
            services.AddSingleton<IConfigurationDatabase, YamlConfigurationDatabase>();
            services.AddSingleton<IConfigurationProviderNotifier, ConfigurationProviderNotifier>();
            services.AddSingleton<IConfigurationProvider, ConfigurationProviderReceiver>();
            services.AddSingleton<IConnectorRegistry, ConnectorRegistry>();
            services.AddSingleton<IFlowActor, FlowActor>();
            services.AddSingleton<EventHub>();
            services.AddSingleton<FlowManager>();
            services.AddSingleton<InternalConnector>();
            services.AddSingleton<IFlowContextFactory>(new GenericFlowContextFactory((actor, flowName) => new JObjectFlowContext(actor, flowName)));
            services.AddHostedService<MainService>();

            // Api
            services.AddSingleton<IApi, InternalApi>();
            services.AddGraphQLServer()
                .AddInMemorySubscriptions()
                .AddType<JsonType>()
                .AddTypeConverter<JObject, JToken>(from => from)
                .AddTypeConverter<JArray, JToken>(from => from)
                .AddTypeConverter<JValue, JToken>(from => from)
                .BindRuntimeType<JObject, JsonType>()
                .BindRuntimeType<JArray, JsonType>()
                .BindRuntimeType<JValue, JsonType>()
                .BindRuntimeType<Unit, AnyType>()
                .BindRuntimeType<object, AnyType>()
                .AddQueryType<Query>()
                .AddType<ConfigurationExtension>()
                .AddType<FlowExtension>()
                .AddType<BlockConfigExtension>()
                .AddMutationType<Mutation>()
                .AddType<DatabaseMutation>()
                .AddSubscriptionType<Subscription>()
                .ModifyRequestOptions(options =>
                {
                    options.IncludeExceptionDetails = true;
                });
        }
    }
}