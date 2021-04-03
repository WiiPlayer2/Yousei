using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;
using YouseiReloaded.Internal;
using System.Reactive.Linq;
using HotChocolate.Types;
using Yousei.Api.Types;

namespace Yousei.Api.Subscriptions
{
    public class Subscription
    {
        [SubscribeAndResolve]
        public IAsyncEnumerable<object> OnTrigger(
            BlockConfigInput config,
            [Service] IFlowActor flowActor)
            => flowActor.GetTrigger(config.Map(), new FlowContext(flowActor, $"<api subscription>"))
                .ToAsyncEnumerable();
    }
}