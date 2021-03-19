using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yousei.Shared;

namespace YouseiReloaded
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
            foreach (var action in actions)
            {
                await Act(action, context);
            }
        }

        public Task Act(BlockConfig action, IFlowContext context)
        {
            var (connectorName, name) = action.Type.SplitType();
            var connector = connectorRegistry.Get(connectorName);

            var connectionConfiguration = configurationProvider
                .GetConnectionConfiguration(connectorName, action.ConfigurationName)
                .Map(connector.ConfigurationType);
            var connection = connector.GetConnection(connectionConfiguration);

            var flowAction = connection.CreateAction(name);
            var flowActionConfiguration = action.Arguments.Map(flowAction.ArgumentsType);

            return flowAction.Act(context, flowActionConfiguration);
        }
    }
}