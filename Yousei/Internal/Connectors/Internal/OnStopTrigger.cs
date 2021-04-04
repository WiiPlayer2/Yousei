using System;
using System.Reactive;
using System.Reactive.Linq;
using Yousei.Core;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class OnStopTrigger : ObservableTrigger
    {
        public OnStopTrigger(IObservable<Unit> stopObservable)
            : base(stopObservable.Select(_ => (object)_))
        {
        }
    }
}