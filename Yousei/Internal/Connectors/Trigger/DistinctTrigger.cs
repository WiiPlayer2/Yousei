using System;
using System.Reactive.Linq;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Trigger
{
    internal class DistinctTrigger : FlowTrigger<UnitConnection, DistinctArguments>
    {
        public override string Name { get; } = "distinct";

        protected override IObservable<object> GetEvents(IFlowContext context, UnitConnection _, DistinctArguments? arguments)
        {
            if (arguments is null || arguments.Base is null)
                throw new ArgumentNullException(nameof(arguments));

            var observable = context.Actor.GetTrigger(arguments.Base, context);
            return observable.DistinctUntilChanged();
        }
    }
}