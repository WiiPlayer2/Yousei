using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using System.Reactive;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class InternalConnector : Connector<Unit>
    {
        private InternalConnector() : base("internal")
        {
        }

        public static InternalConnector Instance { get; } = new();

        protected override IConnection GetConnection(Unit configuration)
            => InternalConnection.Instance;
    }
}