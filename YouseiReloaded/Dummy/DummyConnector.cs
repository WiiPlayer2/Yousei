using System;
using System.Threading.Tasks;
using Yousei.Shared;

namespace YouseiReloaded.Dummy
{
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
}