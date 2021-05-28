using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
                var (connection, name) = GetConnection(trigger, context);

                var flowTrigger = connection.CreateTrigger(name);
                if (flowTrigger is null)
                    throw new FlowException($"Unable to acquire trigger \"{trigger.Type}\".", context);

                var flowTriggerConfiguration = trigger.Arguments.Map(flowTrigger.ArgumentsType);

                return flowTrigger.GetEvents(context, flowTriggerConfiguration);
            }
            catch (Exception e) when (e is not FlowException)
            {
                throw new FlowException($"Error while getting trigger \"{trigger.Type}\".", context, e);
            }
        }

        private async Task Act(BlockConfig action, IFlowContext context)
        {
            try
            {
                var (connection, name) = GetConnection(action, context);

                var flowAction = connection.CreateAction(name);
                if (flowAction is null)
                    throw new FlowException($"Unable to acquire action \"{action.Type}\".", context);

                var flowActionConfiguration = action.Arguments.Map(flowAction.ArgumentsType);

                context.CurrentType = action.Type;
                await flowAction.Act(context, flowActionConfiguration);
            }
            catch (Exception e) when (e is not FlowException)
            {
                throw new FlowException($"Error while executing \"{action.Type}\".", context, e);
            }
        }

        private (IConnection Connection, string Name) GetConnection(BlockConfig config, IFlowContext context)
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

            return (connection, name);
        }
    }
}