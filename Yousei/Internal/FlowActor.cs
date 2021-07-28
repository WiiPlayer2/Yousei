using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Internal;
using Yousei.Shared;

namespace Yousei.Internal
{
    internal class FlowActor : IFlowActor
    {
        private readonly IConfigurationProvider configurationProvider;

        private readonly IConnectorRegistry connectorRegistry;

        public FlowActor(IConfigurationProvider configurationProvider, IConnectorRegistry connectorRegistry)
        {
            this.configurationProvider = configurationProvider;
            this.connectorRegistry = connectorRegistry;
        }

        public async Task Act(IReadOnlyList<BlockConfig> actions, IFlowContext context)
        {
            var currentType = context.CurrentType;
            using (new ActionDisposable(() => context.CurrentType = currentType))
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    var action = actions[i];
                    using (context.ScopeStack($"[{i + 1}] {action.Type}"))
                        await Act(action, context);
                }
            }
        }

        public IObservable<object> GetTrigger(BlockConfig trigger, IFlowContext context)
        {
            try
            {
                var (connector, connection, name) = GetConnection(trigger, context);

                var flowTrigger = connector.GetTrigger(name);
                if (flowTrigger is null)
                    throw new FlowException($"Unable to acquire trigger \"{trigger.Type}\".", context);

                return Observable.DeferAsync(async _ =>
                {
                    var flowTriggerConfiguration = await Resolve(trigger.Arguments, flowTrigger.ArgumentsType, context);

                    return flowTrigger.GetEvents(context, connection, flowTriggerConfiguration);
                });
            }
            catch (Exception e) when (e is not FlowException)
            {
                throw new FlowException($"Error while getting trigger \"{trigger.Type}\".", context, e);
            }
        }

        private static async Task<JToken> Resolve(JToken token, IFlowContext context)
        {
            if (token is JObject jobj)
                return await ResolveObject(jobj);

            if (token is JArray jarr)
                return new JArray(await Task.WhenAll(jarr.Select(o => Resolve(o, context))));

            return token;

            async Task<JToken> ResolveObject(JObject jobject)
            {
                if (jobject.TryToObject<IParameter>(out var parameter))
                {
                    var resolvedValue = await parameter.Resolve(context);
                    return resolvedValue is not null
                        ? JToken.FromObject(resolvedValue)
                        : JValue.CreateNull();
                }

                return new JObject(await Task.WhenAll(jobject.Properties().Select(async prop => new JProperty(prop.Name, await Resolve(prop.Value, context)))));
            }
        }

        private static async Task<object?> Resolve(object? obj, Type targetType, IFlowContext context)
            => obj is not null
                ? (await Resolve(JToken.FromObject(obj), context)).Map(targetType)
                : default;

        private async Task Act(BlockConfig action, IFlowContext context)
        {
            try
            {
                var (connector, connection, name) = GetConnection(action, context);

                var flowAction = connector.GetAction(name);
                if (flowAction is null)
                    throw new FlowException($"Unable to acquire action \"{action.Type}\".", context);

                var flowActionConfiguration = await Resolve(action.Arguments, flowAction.ArgumentsType, context);

                context.CurrentType = action.Type;
                await flowAction.Act(context, connection, flowActionConfiguration);
            }
            catch (Exception e) when (e is not FlowException)
            {
                throw new FlowException($"Error while executing \"{action.Type}\".", context, e);
            }
        }

        private (IConnector Connector, IConnection Connection, string Name) GetConnection(BlockConfig config, IFlowContext context)
        {
            var (connectorName, name) = config.Type.SplitType();
            var connector = connectorRegistry.Get(connectorName);
            if (connector is null)
                throw new FlowException($"Unable to acquire connector \"{connectorName}\".", context);

            var connectionConfiguration = configurationProvider
                .GetConnectionConfiguration(connectorName, config.Configuration)
                .Map(connector.ConfigurationType);
            var connection = connector.GetConnection(connectionConfiguration);
            if (connection is null)
                throw new FlowException($"Unable to acquire connection for \"{connectorName}\" and configuration {{{connectionConfiguration}}}.", context);

            return (connector, connection, name);
        }
    }
}