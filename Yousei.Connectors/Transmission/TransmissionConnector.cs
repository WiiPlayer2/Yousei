using Transmission.API.RPC;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    public class TransmissionConnector : SimpleConnector<TransmissionConfiguration>
    {
        public TransmissionConnector() : base("transmission")
        {
        }

        protected override IConnection CreateConnection(TransmissionConfiguration configuration)
        {
            var client = new Client(
                configuration.Endpoint,
                login: configuration.Login,
                password: configuration.Password
            );

            return new TransmissionConnection(client);
        }
    }
}