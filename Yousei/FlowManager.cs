using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei;
using Yousei.Shared;
using YouseiReloaded.Internal;
using YouseiReloaded.Internal.Connectors.Internal;

namespace Yousei
{
    internal class FlowManager
    {
        private readonly IConfigurationProvider configurationProvider;

        private readonly IConnectorRegistry connectorRegistry;

        private readonly EventHub eventHub;

        private readonly IFlowActor flowActor;

        private readonly Dictionary<string, FlowConfig> flowConfigs = new();

        private readonly Dictionary<string, IDisposable> flowSubscriptions = new();

        private readonly ILogger logger;

        private IDisposable flowSubscription;

        public FlowManager(
            ILogger<FlowManager> logger,
            IConfigurationProvider configurationProvider,
            IFlowActor flowActor,
            IConnectorRegistry connectorRegistry,
            EventHub eventHub)
        {
            this.logger = logger;
            this.configurationProvider = configurationProvider;
            this.flowActor = flowActor;
            this.connectorRegistry = connectorRegistry;
            this.eventHub = eventHub;
            eventHub.Reload.Subscribe(async _ => await Reload());
        }

        public void CancelSubscriptions()
        {
            flowSubscription.Dispose();
            foreach (var subscription in flowSubscriptions.Values)
            {
                subscription.Dispose();
            }
        }

        public void LoadFlows()
        {
            flowSubscription = configurationProvider.GetFlows()
                .Synchronize()
                .Subscribe(tuple =>
                {
                    try
                    {
                        if (tuple.Config is null)
                        {
                            flowConfigs.Remove(tuple.Name);
                            if (flowSubscriptions.TryGetValue(tuple.Name, out var subscription))
                                subscription.Dispose();
                            return;
                        }

                        // TODO: Handle duplicate flows. Duplicate flows would overwrite existing flows
                        if (flowSubscriptions.ContainsKey(tuple.Name))
                        {
                            flowSubscriptions[tuple.Name].Dispose();
                            flowSubscriptions.Remove(tuple.Name);
                        }

                        flowConfigs[tuple.Name] = tuple.Config;
                        if (tuple.Config.Trigger is null)
                            return;

                        var flowContext = new FlowContext(flowActor);
                        var triggerEvents = flowActor.GetTrigger(tuple.Config.Trigger, flowContext);
                        flowSubscriptions[tuple.Name] = triggerEvents.Subscribe(async data =>
                        {
                            try
                            {
                                var flowInstanceContext = flowContext.Clone();
                                await flowInstanceContext.SetData(tuple.Config.Trigger.Type, data);
                                await flowActor.Act(tuple.Config.Actions, flowInstanceContext);
                            }
                            catch (Exception exception)
                            {
                                logger.LogError(exception, "Error while handling flow.");
                                eventHub.RaiseEvent(InternalEvent.Exception, exception);
                            }
                        });
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "Error while creating flow.");
                        eventHub.RaiseEvent(InternalEvent.Exception, exception);
                    }
                });
        }

        public async Task Reload()
        {
            eventHub.RaiseEvent(InternalEvent.Reloading);
            CancelSubscriptions();
            await connectorRegistry.ResetAll();
            LoadFlows();
            eventHub.RaiseEvent(InternalEvent.Reloaded);
        }
    }
}