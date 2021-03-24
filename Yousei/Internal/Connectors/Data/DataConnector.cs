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
    internal class DataConnector : SingletonConnector
    {
        public DataConnector() : base("data")
        {
        }

        protected override IConnection CreateConnection() => new DataConnection();
    }
}