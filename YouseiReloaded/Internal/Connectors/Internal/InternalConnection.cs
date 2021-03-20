using System;
using Yousei.Core;
using System.Reactive.Subjects;
using System.Reactive;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class InternalConnection : SimpleConnection<InternalConnection>
    {
        private readonly ISubject<Exception> exceptionSubject = new Subject<Exception>();

        private readonly ISubject<Unit> startSubject = new AsyncSubject<Unit>();

        private readonly ISubject<Unit> stopSubject = new AsyncSubject<Unit>();

        private readonly ISubject<(string Topic, object Value)> valueSubject = new Subject<(string, object)>();

        private InternalConnection()
        {
            AddTrigger("onexception", new OnExceptionTrigger(exceptionSubject));
            AddTrigger("onvalue", new OnValueTrigger(valueSubject));
            AddTrigger("onstart", new OnStartTrigger(startSubject));
            AddTrigger("onstop", new OnStopTrigger(stopSubject));
            AddAction("sendvalue", new SendValueAction(valueSubject));
        }

        public static InternalConnection Instance { get; } = new();

        public void OnException(Exception exception)
            => exceptionSubject.OnNext(exception);

        public void OnStart()
        {
            startSubject.OnNext(Unit.Default);
            startSubject.OnCompleted();
        }

        public void OnStop()
        {
            stopSubject.OnNext(Unit.Default);
            stopSubject.OnCompleted();
        }
    }
}