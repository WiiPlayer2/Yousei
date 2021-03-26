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
            JToken config,
            [Service] IApi api)
        {
            await api.ConfigurationDatabase.SetConfiguration(connector, name, config);
            return new Configuration(connector, name);
        }

        public async Task<Flow> SetFlow(
            string name,
            FlowConfigInput config,
            [Service] IApi api)
        {
            await api.ConfigurationDatabase.SetFlow(name, config.Map());
            return new Flow(name);
        }
    }
}