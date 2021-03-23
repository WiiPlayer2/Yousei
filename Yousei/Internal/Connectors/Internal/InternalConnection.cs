using System;
using Yousei.Core;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reactive.Linq;
using Yousei;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class InternalConnection : SimpleConnection
    {
        private readonly EventHub eventHub;

        public InternalConnection(EventHub eventHub)
        {
            this.eventHub = eventHub;

            AddTrigger("onevent", new ObservableTrigger(eventHub.Events.Select(o => new
            {
                Event = o.Event,
                Data = o.Data,
            })));
            AddTrigger("onexception", new ObservableTrigger(Filter<Exception>(InternalEvent.Exception)));
            AddTrigger("onstart", new ObservableTrigger(Filter(InternalEvent.Start).FirstAsync()));
            AddTrigger("onstop", new ObservableTrigger(Filter(InternalEvent.Stop).FirstAsync()));

            AddTrigger("onvalue", new OnValueTrigger(eventHub.Values));
            AddAction("sendvalue", new SendValueAction(eventHub.Values));
        }

        private IObservable<object> Filter(InternalEvent @event)
            => Filter<object>(@event);

        private IObservable<T> Filter<T>(InternalEvent @event)
            => eventHub.Events.Where(o => o.Event == @event)
                .Select(o => (T)o.Data);
    }
}