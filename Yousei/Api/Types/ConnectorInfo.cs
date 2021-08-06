using HotChocolate;
using HotChocolate.Configuration;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;

namespace Yousei.Api.Types
{
    public class ConnectorInfo : Wrapper<IConnector>
    {
        public ConnectorInfo(IConnector connector) : base(connector)
        {
        }

        public TypeInfo ConfigurationType => (TypeInfo)Wrapped.ConfigurationType;

        public string Name => Wrapped.Name;

        [return: NotNullIfNotNull("connector")]
        public static ConnectorInfo? From(IConnector? connector)
            => connector is not null
                ? new ConnectorInfo(connector)
                : null;

        public FlowActionInfo? GetAction(string name) => FlowActionInfo.From(Wrapped.GetAction(name));

        public IEnumerable<FlowActionInfo> GetActions() => Wrapped.GetActions().Select(o => FlowActionInfo.From(o));

        public async Task<Configuration?> GetConnection(
            string name,
            [Service] IConfigurationDatabase database)
        {
            var configDict = await database.ListConfigurations();
            if (!configDict.TryGetValue(Name, out var items) || !items.Contains(name))
                return null;
            return new Configuration(Name, name);
        }

        public async Task<IEnumerable<Configuration>> GetConnections(
                    [Service] IConfigurationDatabase database)
        {
            var configDict = await database.ListConfigurations();
            if (!configDict.TryGetValue(Name, out var items))
                return Enumerable.Empty<Configuration>();
            return items.Select(o => new Configuration(Name, o));
        }

        public FlowTriggerInfo? GetTrigger(string name) => FlowTriggerInfo.From(Wrapped.GetTrigger(name));

        public IEnumerable<FlowTriggerInfo> GetTriggers() => Wrapped.GetTriggers().Select(o => FlowTriggerInfo.From(o));
    }
}