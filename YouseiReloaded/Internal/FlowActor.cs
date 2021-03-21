using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal
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
                foreach (var action in actions)
                {
                    await Act(action, context);
                }
            }
        }

        public IObservable<object> GetTrigger(BlockConfig trigger)
        {
            var (connection, name) = GetConnection(trigger);

            var flowTrigger = connection.CreateTrigger(name);
            var flowTriggerConfiguration = trigger.Arguments.Map(flowTrigger.ArgumentsType);

            return flowTrigger.GetEvents(flowTriggerConfiguration);
        }

        private Task Act(BlockConfig action, IFlowContext context)
        {
            var (connection, name) = GetConnection(action);

            var flowAction = connection.CreateAction(name);
            var flowActionConfiguration = action.Arguments.Map(flowAction.ArgumentsType);

            context.CurrentType = action.Type;
            return flowAction.Act(context, flowActionConfiguration);
        }

        private (IConnection Connection, string Name) GetConnection(BlockConfig config)
        {
            var (connectorName, name) = config.Type.SplitType();
            var connector = connectorRegistry.Get(connectorName);

            var connectionConfiguration = configurationProvider
                .GetConnectionConfiguration(connectorName, config.Configuration)
                .Map(connector.ConfigurationType);
            var connection = connector.GetConnection(connectionConfiguration);

            return (connection, name);
        }
    }
}