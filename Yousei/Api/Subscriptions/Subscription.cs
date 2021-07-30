using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;
using Yousei.Internal;
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
            [Service] IFlowActor flowActor,
            [Service] IFlowContextFactory flowContextFactory)
            => flowActor.GetTrigger(config.Map(), flowContextFactory.Create(flowActor, $"<api subscription>"))
                .ToAsyncEnumerable();
    }
}