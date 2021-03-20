using Newtonsoft.Json.Linq;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace YouseiReloaded.Dummy
{
    internal class DummyConnector : IConnector
    {
        public Type ConfigurationType { get; } = typeof(Config);

        public string Name { get; } = "dummy";

        public IConnection GetConnection(object configuration) => new Connection();

        private class Config
        {
        }

        private class Connection : IConnection
        {
            public IFlowAction CreateAction(string name) => new OutAction();

            public IFlowTrigger CreateTrigger(string name) => new Trigger();
        }

        private class OutAction : IFlowAction
        {
            public Type ArgumentsType { get; } = typeof(Arguments);

            public string Type => throw new NotImplementedException();

            public async Task Act(IFlowContext context, object arguments)
            {
                var args = arguments as Arguments;
                Console.WriteLine(await args.Text.Resolve<object>(context));
            }

            private class Arguments
            {
                public IParameter Text { get; init; }
            }
        }

        private class Trigger : IFlowTrigger
        {
            public Type ArgumentsType { get; } = typeof(Arguments);

            public string Type => throw new NotImplementedException();

            public IObservable<JToken> GetEvents(object arguments)
            {
                var args = arguments as Arguments;
                return Observable.Create<JToken>(async (observer, cancellationToken) =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(args.Seconds), cancellationToken);
                        observer.OnNext(JToken.FromObject(DateTimeOffset.Now));
                        observer.OnCompleted();
                    })
                    .Repeat();
            }

            private class Arguments
            {
                public double Seconds { get; init; }
            }
        }
    }
}