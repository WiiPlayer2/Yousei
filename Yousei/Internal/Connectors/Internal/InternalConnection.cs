using System;
using Yousei.Core;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reactive.Linq;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class InternalConnection : SimpleConnection
    {
        private readonly ISubject<(InternalEvent Event, object Data)> eventSubject = new Subject<(InternalEvent, object)>();

        private readonly ISubject<(string Topic, object Value)> valueSubject = new Subject<(string, object)>();

        private InternalConnection()
        {
            AddTrigger("onevent", new ObservableTrigger(eventSubject.Select(o => new
            {
                Event = o.Event,
                Data = o.Data,
            })));
            AddTrigger("onexception", new ObservableTrigger(Filter<Exception>(InternalEvent.Exception)));
            AddTrigger("onstart", new ObservableTrigger(Filter(InternalEvent.Start).FirstAsync()));
            AddTrigger("onstop", new ObservableTrigger(Filter(InternalEvent.Stop).FirstAsync()));

            AddTrigger("onvalue", new OnValueTrigger(valueSubject));
            AddAction("sendvalue", new SendValueAction(valueSubject));
        }

        public static InternalConnection Instance { get; } = new();

        public void RaiseEvent(InternalEvent @event, object data = default)
            => eventSubject.OnNext((@event, data ?? Unit.Default));

        private IObservable<object> Filter(InternalEvent @event)
            => Filter<object>(@event);

        private IObservable<T> Filter<T>(InternalEvent @event)
            => eventSubject.Where(o => o.Event == @event)
                .Select(o => (T)o.Data);
    }
}