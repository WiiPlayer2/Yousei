﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace YouseiReloaded.Internal
{
    internal class ConnectorRegistry : IConnectorRegistry
    {
        private readonly ConcurrentDictionary<string, IConnector> connectors = new();

        public ConnectorRegistry()
        {
            // Load internal connectors
            Register(new Dummy.DummyConnector());
        }

        public IConnector Get(string name) => connectors.GetValueOrDefault(name);

        public void Register(IConnector connector) => connectors.TryAdd(connector.Name, connector);

        public void Unregister(IConnector connector) => connectors.TryRemove(connector.Name, out var _);
    }
}