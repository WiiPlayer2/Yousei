using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei;
using Yousei.Core;
using Yousei.Internal;
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

        private IDisposable? flowSubscription;

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
            flowSubscription?.Dispose();
            foreach (var subscription in flowSubscriptions.Values)
            {
                subscription.Dispose();
            }
        }

        public void LoadFlows()
        {
            flowSubscription = configurationProvider.GetFlows()
                .Subscribe(tuple =>
                {
                    try
                    {
                        if (tuple.Config is null)
                        {
                            if (flowConfigs.Remove(tuple.Name))
                                eventHub.RaiseEvent(InternalEvent.FlowRemoved, tuple.Name);
                            if (flowSubscriptions.TryGetValue(tuple.Name, out var subscription))
                                subscription.Dispose();
                            return;
                        }

                        if (flowSubscriptions.ContainsKey(tuple.Name))
                        {
                            flowSubscriptions[tuple.Name].Dispose();
                            flowSubscriptions.Remove(tuple.Name);
                            eventHub.RaiseEvent(InternalEvent.FlowUpdated, tuple.Name);
                        }
                        else
                        {
                            eventHub.RaiseEvent(InternalEvent.FlowAdded, tuple.Name);
                        }

                        flowConfigs[tuple.Name] = tuple.Config;
                        if (tuple.Config.Trigger is null)
                            return;

                        var flowContext = new FlowContext(flowActor, tuple.Name);
                        flowContext.ExecutionStack.Push($"-> {tuple.Name}");
                        var triggerEvents = flowActor.GetTrigger(tuple.Config.Trigger, flowContext);
                        flowSubscriptions[tuple.Name] = triggerEvents
                            .ObserveOn(TaskPoolScheduler.Default)
                            .Do(
                                onNext: _ => { },
                                onError: exception =>
                                {
                                    var flowException = new FlowException($"Error in subscription from \"{tuple.Config.Trigger.Type}\".", flowContext, exception);
                                    eventHub.RaiseEvent(InternalEvent.Exception, flowException);
                                })
                            .Retry()
                            .Subscribe(async data =>
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
                        logger.LogError(exception, $"Error while creating flow \"{tuple.Name}\".");
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