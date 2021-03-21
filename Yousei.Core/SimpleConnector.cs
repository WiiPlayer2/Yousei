using System.Collections.Generic;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class SimpleConnector<TConfiguration> : Connector<TConfiguration>
    {
        protected readonly IConnection defaultConnection;

        private readonly Dictionary<TConfiguration, IConnection> connections = new();

        protected SimpleConnector(string name) : base(name)
        {
        }

        protected abstract IConnection CreateConnection(TConfiguration configuration);

        protected override IConnection GetConnection(TConfiguration configuration)
        {
            if (configuration is null)
                return defaultConnection;

            if (!connections.TryGetValue(configuration, out var connetion))
            {
                connetion = CreateConnection(configuration);
                connections.Add(configuration, connetion);
            }
            return connetion;
        }
    }
}