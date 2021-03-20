using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class Connector<TConfiguration> : IConnector
    {
        public Connector(string name)
        {
            Name = name;
        }

        public Type ConfigurationType { get; } = typeof(TConfiguration);

        public string Name { get; }

        public IConnection GetConnection(object configuration) => GetConnection(configuration.SafeCast<TConfiguration>());

        protected abstract IConnection GetConnection(TConfiguration configuration);
    }
}