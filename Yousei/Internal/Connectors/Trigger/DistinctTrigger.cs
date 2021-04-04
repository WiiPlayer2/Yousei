using System;
using System.Reactive.Linq;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Trigger
{
    internal class DistinctTrigger : FlowTrigger<DistinctArguments>
    {
        protected override IObservable<object> GetEvents(IFlowContext context, DistinctArguments? arguments)
        {
            if (arguments is null || arguments.Base is null)
                throw new ArgumentNullException(nameof(arguments));

            var observable = context.Actor.GetTrigger(arguments.Base, context);
            return observable.DistinctUntilChanged();
        }
    }
}