using System;
using Yousei.Shared;

namespace YouseiReloaded.Dummy
{
    internal class DummyConnectorRegistry : IConnectorRegistry
    {
        public IConnector Get(string name) => new DummyConnector();

        public void Register(IConnector connector)
        {
            throw new NotImplementedException();
        }

        public void Unregister(IConnector connector)
        {
            throw new NotImplementedException();
        }
    }
}