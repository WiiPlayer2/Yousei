using System.Collections.Generic;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class SimpleConnector<TConnection, TConfiguration> : Connector<TConnection, TConfiguration>
        where TConnection : IConnection
        where TConfiguration : notnull
    {
        private readonly Dictionary<string, IFlowAction> actions = new();

        private readonly Dictionary<TConfiguration, TConnection?> connections = new();

        private readonly Dictionary<string, IFlowTrigger> triggers = new();

        protected TConnection? DefaultConnection { get; set; }

        public sealed override IFlowAction? GetAction(string name)
            => actions.GetValueOrDefault(name);

        public sealed override IFlowTrigger? GetTrigger(string name)
            => triggers.GetValueOrDefault(name);

        public sealed override Task Reset()
        {
            foreach (var connection in connections.Values)
                connection?.Dispose();
            connections.Clear();
            return Task.CompletedTask;
        }

        protected void AddAction<T>()
            where T : IFlowAction, new()
            => AddAction(new T());

        protected void AddAction(IFlowAction instance)
            => actions.Add(instance.Name, instance);

        protected void AddTrigger<T>()
            where T : IFlowTrigger, new()
            => AddTrigger(new T());

        protected void AddTrigger(IFlowTrigger instance)
            => triggers.Add(instance.Name, instance);

        protected abstract TConnection? CreateConnection(TConfiguration configuration);

        protected sealed override TConnection? GetConnection(TConfiguration? configuration)
        {
            if (configuration is null)
                return DefaultConnection;

            if (!connections.TryGetValue(configuration, out var connetion))
            {
                connetion = CreateConnection(configuration);
                connections.Add(configuration, connetion);
            }
            return connetion;
        }
    }
}