using System;
using System.Reactive;
using Yousei.Core;
using System.Reactive.Linq;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class OnStartTrigger : FlowTrigger<Unit>
    {
        private readonly IObservable<Unit> startObservable;

        public OnStartTrigger(IObservable<Unit> startObservable)
        {
            this.startObservable = startObservable;
        }

        protected override IObservable<object> GetEvents(Unit arguments)
            => startObservable.Select(o => (object)o);
    }
}