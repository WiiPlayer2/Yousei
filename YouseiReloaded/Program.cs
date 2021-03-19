using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yousei.Shared;

namespace YouseiReloaded
{
    internal class DummyConfigurationProvider : IConfigurationProvider
    {
        public object GetConnectionConfiguration(string type, string name) => default;

        public FlowConfig GetFlow(string name)
        {
            throw new NotImplementedException();
        }

        public IObservable<(string Name, FlowConfig Config)> GetFlows()
        {
            throw new NotImplementedException();
        }
    }

    internal class DummyConnector : IConnector
    {
        public Type ConfigurationType { get; } = typeof(Config);

        public IConnection GetConnection(object configuration) => new Connection();

        private class Config
        {
        }

        private class Connection : IConnection
        {
            public IFlowAction CreateAction(string name) => new OutAction();

            public IFlowTrigger CreateTrigger(string name)
            {
                throw new NotImplementedException();
            }
        }

        private class OutAction : IFlowAction
        {
            public Type ArgumentsType { get; } = typeof(Arguments);

            public string Type => throw new NotImplementedException();

            public async Task Act(IFlowContext context, object arguments)
            {
                var args = arguments as Arguments;
                Console.WriteLine(await args.Text.Resolve<string>(context));
            }

            private class Arguments
            {
                public IParameter Text { get; init; }
            }
        }
    }

    internal class DummyConnectorRegistry : IConnectorRegistry
    {
        public IConnector Get(string name) => new DummyConnector();

        public void Register(IConnector connector)
        {
            throw new NotImplementedException();
        }

        public void Unregister(IConnector connector)
        {
            throw new NotImplementedException();
        }
    }

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new[]
                {
                    new ParameterConverter(),
                }
            };

            var blockConfigs = new BlockConfig[]
            {
                new()
                {
                    Type = "console.out",
                    Arguments = new Dictionary<string, object>
                    {
                        { "text", new ExpressionParameter(@"$""asdf: {Context.http.webhook}""") },
                    },
                }
            };

            var actor = new FlowActor(new DummyConfigurationProvider(), new DummyConnectorRegistry());
            var context = new FlowContext(actor);
            await context.AddData("http.webhook", JToken.FromObject("this is definitely a http body"));
            await actor.Act(blockConfigs, context);
        }
    }
}