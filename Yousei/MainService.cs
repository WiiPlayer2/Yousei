using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Shared;
using YouseiReloaded.Internal;
using YouseiReloaded.Internal.Connectors.Internal;

namespace YouseiReloaded
{
    internal class MainService : IHostedService
    {
        private readonly IConfigurationProvider configurationProvider;

        private readonly IConnectorRegistry connectorRegistry;

        private readonly IFlowActor flowActor;

        private readonly Dictionary<string, FlowConfig> flowConfigs = new();

        private readonly Dictionary<string, IDisposable> flowSubscriptions = new();

        private readonly ILogger<MainService> logger;

        private IDisposable flowSubscription;

        public MainService(ILogger<MainService> logger, IConfigurationProvider configurationProvider, IFlowActor flowActor, IConnectorRegistry connectorRegistry)
        {
            this.logger = logger;
            this.configurationProvider = configurationProvider;
            this.flowActor = flowActor;
            this.connectorRegistry = connectorRegistry;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            LoadFlows();
            InternalConnection.Instance.RaiseEvent(InternalEvent.Start);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            InternalConnection.Instance.RaiseEvent(InternalEvent.Stop);
            CancelSubscriptions();
            return Task.CompletedTask;
        }

        private void CancelSubscriptions()
        {
            flowSubscription.Dispose();
            foreach (var subscription in flowSubscriptions.Values)
            {
                subscription.Dispose();
            }
        }

        private void LoadFlows()
        {
            flowSubscription = configurationProvider.GetFlows()
                .Synchronize()
                .Subscribe(tuple =>
                {
                    try
                    {
                        if (tuple.Config is null && flowConfigs.ContainsKey(tuple.Name))
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
                                InternalConnection.Instance.RaiseEvent(InternalEvent.Exception, exception);
                            }
                        });
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "Error while creating flow.");
                        InternalConnection.Instance.RaiseEvent(InternalEvent.Exception, exception);
                    }
                });
        }

        private async Task Reload()
        {
            InternalConnection.Instance.RaiseEvent(InternalEvent.Reloading);
            CancelSubscriptions();
            await connectorRegistry.ResetAll();
            LoadFlows();
            InternalConnection.Instance.RaiseEvent(InternalEvent.Reloaded);
        }
    }
}