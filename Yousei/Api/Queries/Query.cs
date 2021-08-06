using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;
using TypeInfo = Yousei.Api.Types.TypeInfo;

namespace Yousei.Api.Queries
{
    public class Query
    {
        public Database Database { get; } = new();

        public ConnectorInfo? GetConnector(
            string name,
            [Service] IConnectorRegistry connectorRegistry)
            => ConnectorInfo.From(connectorRegistry.Get(name));

        public IEnumerable<ConnectorInfo> GetConnectors(
            [Service] IConnectorRegistry connectorRegistry)
            => connectorRegistry.GetAll().Select(o => ConnectorInfo.From(o));

        public TypeInfo? GetType(string name)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var type = loadedAssemblies.Select(a => a.GetType(name)).FirstOrDefault(o => o is not null);
            return type is not null
                ? (TypeInfo)type
                : null;
        }
    }
}