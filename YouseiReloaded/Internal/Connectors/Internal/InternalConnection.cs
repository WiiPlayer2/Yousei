using System;
using Yousei.Core;
using System.Reactive.Subjects;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class InternalConnection : SimpleConnection<InternalConnection>
    {
        private readonly ISubject<Exception> exceptionSubject = new Subject<Exception>();

        private readonly ISubject<(string Topic, object Value)> valueSubject = new Subject<(string, object)>();

        private InternalConnection()
        {
            AddTrigger("onexception", new OnExceptionTrigger(exceptionSubject));
            AddTrigger("onvalue", new OnValueTrigger(valueSubject));
            AddAction("sendvalue", new SendValueAction(valueSubject));
        }

        public static InternalConnection Instance { get; } = new();

        public void OnException(Exception exception)
            => exceptionSubject.OnNext(exception);
    }
}