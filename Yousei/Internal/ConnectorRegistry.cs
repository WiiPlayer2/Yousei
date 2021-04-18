using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Connectors.Http;
using Yousei.Connectors.Telegram;
using Yousei.Connectors.Transmission;
using Yousei.Connectors.Rss;
using Yousei.Shared;
using YouseiRelaoded.Internal.Connectors.Log;
using Yousei.Internal.Connectors.Control;
using Yousei.Internal.Connectors.Data;
using Yousei.Internal.Connectors.Internal;
using Yousei.Internal.Connectors.Trigger;
using Yousei.Internal.Connectors.Debug;

namespace Yousei.Internal
{
    internal class ConnectorRegistry : IConnectorRegistry
    {
        private readonly ConcurrentDictionary<string, IConnector> connectors = new();

        public ConnectorRegistry(
            ILogger<LogConnector> logConnectorLogger,
            ILogger<DebugConnector> debugConnectorLogger,
            InternalConnector internalConnector)
        {
            // Internal connectors
            Register(internalConnector);
            Register(new ControlConnector());
            Register(new DataConnector());
            Register(new LogConnector(logConnectorLogger));
            Register(new TriggerConnector());
            Register(new DebugConnector(debugConnectorLogger));

            // External connectors
            Register(new HttpConnector());
            Register(new TransmissionConnector());
            Register(new TelegramConnector());
            Register(new RssConnector());
        }

        public IConnector? Get(string name) => connectors.GetValueOrDefault(name);

        public void Register(IConnector connector) => connectors.TryAdd(connector.Name, connector);

        public Task ResetAll() => Task.WhenAll(connectors.Values.Select(o => o.Reset()));

        public void Unregister(IConnector connector) => connectors.TryRemove(connector.Name, out var _);
    }
}