using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class SingletonConnector : SingletonConnector<UnitConnection>
    {
        protected sealed override UnitConnection CreateConnection()
            => UnitConnection.Default;
    }

    public abstract class SingletonConnector<TConnection> : SimpleConnector<TConnection, Unit>
        where TConnection : IConnection
    {
        private readonly Lazy<TConnection> lazyConnection;

        protected SingletonConnector()
        {
            lazyConnection = new Lazy<TConnection>(CreateConnection);
        }

        protected override TConnection? CreateConnection(Unit configuration)
            => lazyConnection.Value;

        protected abstract TConnection CreateConnection();
    }
}