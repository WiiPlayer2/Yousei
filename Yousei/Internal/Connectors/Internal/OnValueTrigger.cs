using System;
using Yousei.Core;
using System.Reactive.Linq;
using Yousei.Shared;
using System.Reactive;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class OnValueTrigger : FlowTrigger<OnValueConfiguration>
    {
        public readonly IObservable<(string Topic, object? Value)> valueObservable;

        public OnValueTrigger(IObservable<(string, object?)> valueObservable)
        {
            this.valueObservable = valueObservable;
        }

        protected override IObservable<object> GetEvents(IFlowContext context, OnValueConfiguration? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            return valueObservable
                .Where(o => o.Topic == arguments.Topic)
                .Select(o => o.Value ?? Unit.Default);
        }
    }
}