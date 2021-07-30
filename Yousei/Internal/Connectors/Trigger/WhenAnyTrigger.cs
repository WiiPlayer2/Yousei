using System;
using System.Linq;
using System.Reactive.Linq;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Trigger
{
    internal class WhenAnyTrigger : FlowTrigger<UnitConnection, WhenAnyArguments>
    {
        public override string Name { get; } = "whenany";

        protected override IObservable<object> GetEvents(IFlowContext context, UnitConnection _, WhenAnyArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            return Observable.Defer(() =>
                {
                    var observables = arguments.Triggers
                        .Select(trigger => context.Actor.GetTrigger(trigger, context)
                            .Select(o => new
                            {
                                Source = trigger.Type,
                                Data = o,
                            }));
                    return observables.Merge();
                });
        }
    }
}