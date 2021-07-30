using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Yousei.Internal.Connectors.Internal;

namespace Yousei
{
    internal class EventHub
    {
        public ISubject<(InternalEvent Event, object Data)> Events { get; } = new Subject<(InternalEvent, object)>();

        public ISubject<Unit> Reload { get; } = new Subject<Unit>();

        public ISubject<(string Topic, object? Value)> Values { get; } = new Subject<(string, object?)>();

        public void RaiseEvent(InternalEvent @event, object? data = default)
            => Events.OnNext((@event, data ?? Unit.Default));

        public void TriggerReload()
            => Reload.OnNext(Unit.Default);
    }
}