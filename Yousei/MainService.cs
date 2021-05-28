using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei;
using Yousei.Internal.Connectors.Internal;

namespace Yousei
{
    internal class MainService : IHostedService
    {
        private readonly EventHub eventHub;

        private readonly FlowManager flowManager;

        private readonly ILogger<MainService> logger;

        public MainService(FlowManager flowManager, EventHub eventHub, ILogger<MainService> logger)
        {
            this.flowManager = flowManager;
            this.eventHub = eventHub;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            AppDomain.CurrentDomain.FirstChanceException += (_, e) => logger.LogDebug($"First chance: {e.Exception}");
            AppDomain.CurrentDomain.UnhandledException += (_, e) => logger.LogCritical($"Unhandled{(e.IsTerminating ? " (terminating)" : string.Empty)}: {e.ExceptionObject}");

            flowManager.LoadFlows();
            eventHub.RaiseEvent(InternalEvent.Start);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            eventHub.RaiseEvent(InternalEvent.Stop);
            flowManager.CancelSubscriptions();
            return Task.CompletedTask;
        }
    }
}