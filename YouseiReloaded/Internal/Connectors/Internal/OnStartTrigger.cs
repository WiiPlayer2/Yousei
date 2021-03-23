using System;
using System.Reactive;
using Yousei.Core;
using System.Reactive.Linq;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class OnStartTrigger : ObservableTrigger
    {
        public OnStartTrigger(IObservable<Unit> startObservable)
            : base(startObservable.Select(_ => default(object)))
        {
        }
    }
}