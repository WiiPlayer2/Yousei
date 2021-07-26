using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class Connector<TConnection, TConfiguration> : IConnector
        where TConnection : IConnection
    {
        public Connector(string name)
        {
            Name = name;
        }

        public Type ConfigurationType { get; } = typeof(TConfiguration);

        public string Name { get; }

        public abstract IFlowAction? GetAction(string name);

        public IConnection? GetConnection(object? configuration)
            => GetConnection(configuration.SafeCast<TConfiguration>());

        public abstract IFlowTrigger? GetTrigger(string name);

        public abstract Task Reset();

        protected abstract TConnection? GetConnection(TConfiguration? configuration);
    }
}