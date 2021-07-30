using System;
using System.Linq;
using System.Reactive.Linq;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Internal
{
    internal class InternalConnector : SingletonConnector
    {
        private readonly EventHub eventHub;

        public InternalConnector(EventHub eventHub)
        {
            this.eventHub = eventHub;

            AddTrigger(new ObservableTrigger("onevent", eventHub.Events.Select(o => new
            {
                Event = o.Event,
                Data = o.Data,
            })));
            AddTrigger(new ObservableTrigger("onexception", Filter<Exception>(InternalEvent.Exception)));
            AddTrigger(new ObservableTrigger("onstart", Filter(InternalEvent.Start).FirstAsync()));
            AddTrigger(new ObservableTrigger("onstop", Filter(InternalEvent.Stop).FirstAsync()));

            AddTrigger(new OnValueTrigger(eventHub.Values));
            AddAction(new SendValueAction(eventHub.Values));

            AddAction(new InvokeAction("reload", eventHub.TriggerReload));
        }

        public override string Name { get; } = "internal";

        private IObservable<object> Filter(InternalEvent @event)
                            => Filter<object>(@event);

        private IObservable<T> Filter<T>(InternalEvent @event)
            => eventHub.Events.Where(o => o.Event == @event)
                .Select(o => (T)o.Data);
    }
}