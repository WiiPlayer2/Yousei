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
using Yousei.Connectors.Imap;
using Yousei.Connectors.Nuxeo;

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
            Register(new LogConnector(logConnectorLogger));
            Register(new DebugConnector(debugConnectorLogger));
            Register<ControlConnector>();
            Register<DataConnector>();
            Register<TriggerConnector>();

            // External connectors
            Register<HttpConnector>();
            Register<TransmissionConnector>();
            Register<TelegramConnector>();
            Register<RssConnector>();
            Register<ImapConnector>();
            Register<NuxeoConnector>();
        }

        public IConnector? Get(string name) => connectors.GetValueOrDefault(name);

        public IEnumerable<IConnector> GetAll() => connectors.Values;

        public void Register(IConnector connector) => connectors.TryAdd(connector.Name, connector);

        public void Register<T>()
            where T : IConnector, new()
            => Register(new T());

        public Task ResetAll() => Task.WhenAll(connectors.Values.Select(o => o.Reset()));

        // TODO should make it return a task
        public async void Unregister(IConnector connector)
        {
            if (connectors.TryRemove(connector.Name, out var _))
                await (connector?.Reset() ?? Task.CompletedTask);
        }
    }
}