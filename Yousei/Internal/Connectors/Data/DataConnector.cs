using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Data
{
    internal class DataConnector : Connector<Unit>
    {
        private readonly DataConnection connection = new DataConnection();

        public DataConnector() : base("data")
        {
        }

        protected override IConnection GetConnection(Unit configuration) => connection;
    }
}