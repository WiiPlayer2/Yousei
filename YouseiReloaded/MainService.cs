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

        private readonly IFlowActor flowActor;

        private readonly Dictionary<string, FlowConfig> flowConfigs = new();

        private readonly Dictionary<string, IDisposable> flowSubscriptions = new();

        private readonly ILogger<MainService> logger;

        private IDisposable flowSubscription;

        public MainService(ILogger<MainService> logger, IConfigurationProvider configurationProvider, IFlowActor flowActor)
        {
            this.logger = logger;
            this.configurationProvider = configurationProvider;
            this.flowActor = flowActor;
        }

        public Task StartAsync(CancellationToken cancellationToken)
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

                        var triggerEvents = flowActor.GetTrigger(tuple.Config.Trigger);
                        flowSubscriptions[tuple.Name] = triggerEvents.Subscribe(async data =>
                        {
                            try
                            {
                                var context = new FlowContext(flowActor);
                                await context.SetData(tuple.Config.Trigger.Type, data);
                                await flowActor.Act(tuple.Config.Actions, context);
                            }
                            catch (Exception exception)
                            {
                                logger.LogError(exception, "Error while handling flow.");
                                InternalConnection.Instance.OnException(exception);
                            }
                        });
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "Error while creating flow.");
                        InternalConnection.Instance.OnException(exception);
                    }
                });
            InternalConnection.Instance.OnStart();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            InternalConnection.Instance.OnStop();
            flowSubscription.Dispose();
            foreach (var subscription in flowSubscriptions.Values)
            {
                subscription.Dispose();
            }
            return Task.CompletedTask;
        }
    }
}