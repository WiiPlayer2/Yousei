using System;
using Yousei.Core;
using System.Reactive.Linq;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class OnValueTrigger : FlowTrigger<OnValueConfiguration>
    {
        public readonly IObservable<(string Topic, object Value)> valueObservable;

        public OnValueTrigger(IObservable<(string, object)> valueObservable)
        {
            this.valueObservable = valueObservable;
        }

        protected override IObservable<object> GetEvents(OnValueConfiguration arguments)
            => valueObservable
                .Where(o => o.Topic == arguments.Topic)
                .Select(o => o.Value);
    }
}