using Newtonsoft.Json.Linq;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Dummy
{
    internal class DummyConnector : Connector<Unit>
    {
        public DummyConnector() : base("dummy")
        {
        }

        protected override IConnection GetConnection(Unit _) => new Connection();

        private class Connection : SimpleConnection<Connection>
        {
            public Connection()
            {
                AddTrigger<Trigger>("trigger");
                AddAction<OutAction>("out");
            }
        }

        private class OutAction : FlowAction<OutArguments>
        {
            protected override async Task Act(IFlowContext context, OutArguments arguments)
                => Console.WriteLine(await arguments.Text.Resolve<object>(context));
        }

        private class OutArguments
        {
            public IParameter Text { get; init; }
        }

        private class Trigger : FlowTrigger<TriggerArguments>
        {
            protected override IObservable<JToken> GetEvents(TriggerArguments arguments)
                => Observable.Create<JToken>(async (observer, cancellationToken) =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(arguments.Seconds), cancellationToken);
                        observer.OnNext(JToken.FromObject(DateTimeOffset.Now));
                        observer.OnCompleted();
                    })
                    .Repeat();
        }

        private class TriggerArguments
        {
            public double Seconds { get; init; }
        }
    }
}