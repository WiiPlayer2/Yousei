using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Connectors.Telegram;
using Yousei.Connectors.Rss;
using Yousei.Shared;
using YouseiRelaoded.Internal.Connectors.Log;
using YouseiReloaded.Internal.Connectors.Control;
using YouseiReloaded.Internal.Connectors.Data;
using YouseiReloaded.Internal.Connectors.Internal;

namespace YouseiReloaded.Internal
{
    internal class ConnectorRegistry : IConnectorRegistry
    {
        private readonly ConcurrentDictionary<string, IConnector> connectors = new();

        public ConnectorRegistry(ILogger<LogConnector> logConnectorLogger)
        {
            // Internal connectors
            Register(InternalConnector.Instance);
            Register(new ControlConnector());
            Register(new DataConnector());
            Register(new LogConnector(logConnectorLogger));

            // External connectors
            Register(new TelegramConnector());
            Register(new RssConnector());
        }

        public IConnector Get(string name) => connectors.GetValueOrDefault(name);

        public void Register(IConnector connector) => connectors.TryAdd(connector.Name, connector);

        public void Unregister(IConnector connector) => connectors.TryRemove(connector.Name, out var _);
    }
}