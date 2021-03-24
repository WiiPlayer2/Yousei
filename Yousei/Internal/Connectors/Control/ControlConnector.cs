using System.Reactive;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal class ControlConnector : SingletonConnector
    {
        public ControlConnector() : base("control")
        {
        }

        protected override IConnection CreateConnection() => new ControlConnection();
    }
}