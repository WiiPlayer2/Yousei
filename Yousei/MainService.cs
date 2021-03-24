using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei;
using YouseiReloaded.Internal.Connectors.Internal;

namespace Yousei
{
    internal class MainService : IHostedService
    {
        private readonly EventHub eventHub;

        private readonly FlowManager flowManager;

        public MainService(FlowManager flowManager, EventHub eventHub)
        {
            this.flowManager = flowManager;
            this.eventHub = eventHub;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
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