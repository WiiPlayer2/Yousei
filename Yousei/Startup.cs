using HotChocolate.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Reactive;
using Yousei.Api.Extensions;
using Yousei.Api.Mutations;
using Yousei.Api.Queries;
using Yousei.Api.SchemaType;
using Yousei.Api.Subscriptions;
using Yousei.Api.Types;
using Yousei.Internal;
using Yousei.Internal.Connectors.Internal;
using Yousei.Internal.Database;
using Yousei.Shared;
using CLRPropertyInfo = System.Reflection.PropertyInfo;

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

                // JSON types
                .AddType<JsonType>()
                .AddTypeConverter<JObject, JToken>(from => from)
                .AddTypeConverter<JArray, JToken>(from => from)
                .AddTypeConverter<JValue, JToken>(from => from)
                .BindRuntimeType<JObject, JsonType>()
                .BindRuntimeType<JArray, JsonType>()
                .BindRuntimeType<JValue, JsonType>()

                // Misc. types
                .BindRuntimeType<Unit, AnyType>()
                .BindRuntimeType<object, AnyType>()
                .AddType<SubTypeInfoType<ObjectTypeInfo>>()
                .AddType<SubTypeInfoType<ListTypeInfo>>()
                .AddType<SubTypeInfoType<AnyTypeInfo>>()
                .AddType<SubTypeInfoType<DictionaryTypeInfo>>()
                .AddType<SubTypeInfoType<ScalarTypeInfo>>()
                .AddType<WrapperType<CLRPropertyInfo, PropertyInfo>>()
                .AddType<WrapperType<IConnector, ConnectorInfo>>()
                .AddType<WrapperType<IFlowAction, FlowActionInfo>>()
                .AddType<WrapperType<IFlowTrigger, FlowTriggerInfo>>()

                // Query
                .AddQueryType<Query>()
                .AddType<ConfigurationExtension>()
                .AddType<FlowExtension>()
                .AddType<BlockConfigExtension>()

                // Mutation
                .AddMutationType<Mutation>()
                .AddType<DatabaseMutation>()

                // Subscriptions
                .AddSubscriptionType<Subscription>()

                // Misc.
                .ModifyRequestOptions(options =>
                {
                    options.IncludeExceptionDetails = true;
                });
        }
    }
}