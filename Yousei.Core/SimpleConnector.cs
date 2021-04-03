using System.Collections.Generic;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class SimpleConnector<TConfiguration> : Connector<TConfiguration>
        where TConfiguration : notnull
    {
        private readonly Dictionary<TConfiguration, IConnection?> connections = new();

        protected SimpleConnector(string name) : base(name)
        {
        }

        protected IConnection? DefaultConnection { get; set; }

        public override Task Reset()
        {
            // TODO: maybe force subclass to check each connection if it needs to be handled.
            connections.Clear();
            return Task.CompletedTask;
        }

        protected abstract IConnection? CreateConnection(TConfiguration configuration);

        protected override IConnection? GetConnection(TConfiguration? configuration)
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