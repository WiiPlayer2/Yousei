using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using System.Reactive;
using Yousei.Shared;
using Yousei;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class InternalConnector : SingletonConnector
    {
        private readonly EventHub eventHub;

        public InternalConnector(EventHub eventHub) : base("internal")
        {
            this.eventHub = eventHub;
        }

        protected override IConnection CreateConnection() => new InternalConnection(eventHub);
    }
}