using System;
using Transmission.API.RPC;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    public class TransmissionConnector : SimpleConnector<TransmissionConfiguration>
    {
        public TransmissionConnector()
        {
            AddAction<AddAction>();
            AddAction<GetAction>();
            AddAction<RemoveAction>();
        }

        public override string Name { get; } = "transmission";

        protected override IConnection CreateConnection(TransmissionConfiguration configuration)
        {
            if (configuration.Endpoint is null)
                throw new ArgumentNullException(nameof(configuration.Endpoint));

            var client = new Client(
                configuration.Endpoint.ToString(),
                login: configuration.Login,
                password: configuration.Password
            );

            return ObjectConnection.From(client);
        }
    }
}