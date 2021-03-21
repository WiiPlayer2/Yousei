using System.Reactive;
using Transmission.API.RPC;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    public class TransmissionConnector : Connector<Unit>
    {
        private readonly TransmissionConnection connection;

        public TransmissionConnector(Client client) : base("transmission")
        {
            connection = new TransmissionConnection(client);
        }

        protected override IConnection GetConnection(Unit configuration) => connection;
    }
}