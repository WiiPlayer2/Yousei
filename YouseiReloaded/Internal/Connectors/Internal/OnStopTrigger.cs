using System;
using System.Reactive;
using System.Reactive.Linq;
using Yousei.Core;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class OnStopTrigger : FlowTrigger<Unit>
    {
        private readonly IObservable<Unit> stopObservable;

        public OnStopTrigger(IObservable<Unit> stopObservable)
        {
            this.stopObservable = stopObservable;
        }

        protected override IObservable<object> GetEvents(Unit arguments)
            => stopObservable.Select(o => (object)o);
    }
}