using Transmission.API.RPC;
using Yousei.Core;

namespace Yousei.Connectors.Transmission
{
    internal class TransmissionConnection : SimpleConnection<TransmissionConnection>
    {
        public TransmissionConnection(Client client)
        {
            AddAction("add", new AddAction(client));
            AddAction("get", new GetAction(client));
            AddAction("remove", new RemoveAction(client));
        }
    }
}