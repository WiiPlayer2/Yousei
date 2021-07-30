using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class Connector<TConnection, TConfiguration> : IConnector
        where TConnection : IConnection
    {
        public Type ConfigurationType { get; } = typeof(TConfiguration);

        public abstract string Name { get; }

        public abstract IFlowAction? GetAction(string name);

        public abstract IEnumerable<IFlowAction> GetActions();

        public IConnection? GetConnection(object? configuration)
            => GetConnection(configuration.SafeCast<TConfiguration>());

        public abstract IFlowTrigger? GetTrigger(string name);

        public abstract IEnumerable<IFlowTrigger> GetTriggers();

        public abstract Task Reset();

        protected abstract TConnection? GetConnection(TConfiguration? configuration);
    }
}