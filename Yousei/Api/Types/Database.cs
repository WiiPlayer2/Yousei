using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Api.Types
{
    public class Database
    {
        public async Task<Configuration?> GetConfiguration(
            string connector,
            string name,
            [Service] IConfigurationDatabase configurationDatabase)
            => (await configurationDatabase.GetConfigurationSource(connector, name)) is null
                ? null
                : new Configuration(connector, name);

        public async Task<IEnumerable<Connection>> GetConnections(
            [Service] IConfigurationDatabase configurationDatabase)
            => (await configurationDatabase.ListConfigurations())
                .Select(o => new Connection(o.Key, o.Value.Select(value => new Configuration(o.Key, value)).ToList()));

        public async Task<Flow?> GetFlow(
            string name,
            [Service] IConfigurationDatabase configurationDatabase)
            => (await configurationDatabase.GetFlowSource(name)) is null
                ? null
                : new Flow(name);

        public async Task<IEnumerable<Flow>> GetFlows(
            [Service] IConfigurationDatabase configurationDatabase)
            => (await configurationDatabase.ListFlows())
                .Select(o => new Flow(o));

        public bool GetIsReadOnly(
            [Service] IConfigurationDatabase configurationDatabase)
            => configurationDatabase.IsReadOnly;
    }
}