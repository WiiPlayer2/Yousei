using Transmission.API.RPC;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    public class TransmissionConnector : Connector<TransmissionConfiguration>
    {
        private TransmissionConnection connection = null;

        public TransmissionConnector() : base("transmission")
        {
        }

        protected override IConnection GetConnection(TransmissionConfiguration configuration)
        {
            if (connection is null)
            {
                var client = new Client(
                    configuration.Endpoint,
                    login: configuration.Login,
                    password: configuration.Password
                );
                connection = new TransmissionConnection(client);
            }
            return connection;
        }
    }
}