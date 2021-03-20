using System.Reactive;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal class ControlConnector : Connector<Unit>
    {
        private readonly ControlConnection connection = new ControlConnection();

        public ControlConnector() : base("control")
        {
        }

        protected override IConnection GetConnection(Unit configuration) => connection;
    }
}