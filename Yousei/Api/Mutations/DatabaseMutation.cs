using HotChocolate;
using HotChocolate.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;

namespace Yousei.Api.Mutations
{
    [ExtendObjectType(nameof(Mutation))]
    public class DatabaseMutation
    {
        public async Task<Configuration> SetConfiguration(
            string connector,
            string name,
            SourceConfig source,
            [Service] IApi api)
        {
            await api.ConfigurationDatabase.SetConfiguration(connector, name, source);
            return new Configuration(connector, name);
        }

        public async Task<Flow> SetFlow(
            string name,
            SourceConfig source,
            [Service] IApi api)
        {
            await api.ConfigurationDatabase.SetFlow(name, source);
            return new Flow(name);
        }
    }
}