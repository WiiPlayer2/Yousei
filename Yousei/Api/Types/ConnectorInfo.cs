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

        public TypeInfo ConfigurationType => Wrapped.ConfigurationType;

        public string Name => Wrapped.Name;

        [return: NotNullIfNotNull("connector")]
        public static ConnectorInfo? From(IConnector? connector)
            => connector is not null
                ? new ConnectorInfo(connector)
                : null;

        public FlowActionInfo? GetAction(string name) => FlowActionInfo.From(Wrapped.GetAction(name));

        public FlowTriggerInfo? GetTrigger(string name) => FlowTriggerInfo.From(Wrapped.GetTrigger(name));
    }
}