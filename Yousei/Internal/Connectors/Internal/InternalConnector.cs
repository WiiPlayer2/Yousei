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
    internal class InternalConnector : SingletonConnector
    {
        private InternalConnector() : base("internal")
        {
        }

        public static InternalConnector Instance { get; } = new();

        protected override IConnection CreateConnection() => InternalConnection.Instance;
    }
}