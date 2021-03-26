using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;

namespace Yousei.Api
{
    public class Database
    {
        private readonly IApi api;

        public Database(IApi api)
        {
            this.api = api;
        }

        public bool IsReadOnly => api.ConfigurationDatabase.IsReadOnly;

        public async Task<IEnumerable<Connection>> GetConnections()
            => (await api.ConfigurationDatabase.ListConfigurations())
                .Select(o => new Connection(o.Key, o.Value.Select(value => new Configuration(o.Key, value)).ToList()));

        public async Task<IEnumerable<Flow>> GetFlows()
            => (await api.ConfigurationDatabase.ListFlows())
                .Select(o => new Flow(o));
    }
}