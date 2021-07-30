using System;
using Yousei.Core;
using System.Reactive.Linq;
using Yousei.Shared;
using System.Reactive;

namespace Yousei.Internal.Connectors.Internal
{
    internal class OnValueTrigger : FlowTrigger<UnitConnection, OnValueConfiguration>
    {
        public readonly IObservable<(string Topic, object? Value)> valueObservable;

        public OnValueTrigger(IObservable<(string, object?)> valueObservable)
        {
            this.valueObservable = valueObservable;
        }

        public override string Name { get; } = "onvalue";

        protected override IObservable<object> GetEvents(IFlowContext context, UnitConnection _, OnValueConfiguration? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            return valueObservable
                .Where(o => o.Topic == arguments.Topic)
                .Select(o => o.Value ?? Unit.Default);
        }
    }
}