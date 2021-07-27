using System;
using Yousei.Core;
using System.Reactive.Linq;
using System.Linq;
using Yousei.Shared;

namespace Yousei.Connectors.Http
{
    internal class WebhookTrigger : FlowTrigger<HttpConnection, WebhookArguments>
    {
        public override string Name { get; } = "webhook";

        protected override IObservable<object> GetEvents(IFlowContext context, HttpConnection connection, WebhookArguments? arguments)
            => connection.HttpRequests
                .Where(o => string.IsNullOrEmpty(arguments?.Path) || (o.Url?.AbsolutePath.StartsWith(arguments.Path) ?? false));
    }
}