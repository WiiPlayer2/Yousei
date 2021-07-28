using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;

namespace Yousei.Api.Queries
{
    public class Query
    {
        public Query(IApi api)
        {
            Database = new Database(api);
        }

        public Database Database { get; }

        public ConnectorInfo? GetConnector(
            string name,
            [Service] IConnectorRegistry connectorRegistry)
            => ConnectorInfo.From(connectorRegistry.Get(name));

        public IEnumerable<ConnectorInfo> GetConnectors(
            [Service] IConnectorRegistry connectorRegistry)
            => connectorRegistry.GetAll().Select(o => ConnectorInfo.From(o));
    }
}