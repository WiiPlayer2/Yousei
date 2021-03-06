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
    public class FlowTriggerInfo : Wrapper<IFlowTrigger>
    {
        public FlowTriggerInfo(IFlowTrigger wrapped) : base(wrapped)
        {
        }

        public TypeInfo ArgumentsType => Wrapped.ArgumentsType;

        public string Name => Wrapped.Name;

        [return: NotNullIfNotNull("trigger")]
        public static FlowTriggerInfo? From(IFlowTrigger? trigger)
           => trigger is not null
               ? new FlowTriggerInfo(trigger)
               : null;
    }
}