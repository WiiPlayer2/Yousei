using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class SingletonConnector : IConnector
    {
        private readonly Lazy<IConnection> lazyConnection;

        protected SingletonConnector(string name)
        {
            Name = name;
            lazyConnection = new Lazy<IConnection>(CreateConnection);
        }

        public Type ConfigurationType { get; } = typeof(object);

        public string Name { get; }

        public IConnection GetConnection(object configuration)
            => lazyConnection.Value;

        protected abstract IConnection CreateConnection();
    }
}