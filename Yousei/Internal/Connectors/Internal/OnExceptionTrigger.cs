using System;
using Yousei.Core;
using System.Reactive;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class OnExceptionTrigger : ObservableTrigger
    {
        public OnExceptionTrigger(IObservable<Exception> exceptionObservable)
            : base(exceptionObservable)
        {
        }
    }
}