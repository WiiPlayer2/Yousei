using System;
using Yousei.Core;
using System.Reactive;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class OnExceptionTrigger : FlowTrigger<Unit>
    {
        private readonly IObservable<Exception> exceptionObservable;

        public OnExceptionTrigger(IObservable<Exception> exceptionObservable)
        {
            this.exceptionObservable = exceptionObservable;
        }

        protected override IObservable<object> GetEvents(Unit arguments)
            => exceptionObservable;
    }
}