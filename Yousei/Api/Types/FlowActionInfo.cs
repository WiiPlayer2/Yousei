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
    public class FlowActionInfo : Wrapper<IFlowAction>
    {
        public FlowActionInfo(IFlowAction wrapped) : base(wrapped)
        {
        }

        public TypeInfo ArgumentsType => Wrapped.ArgumentsType;

        public string Name => Wrapped.Name;

        [return: NotNullIfNotNull("action")]
        public static FlowActionInfo? From(IFlowAction? action)
           => action is not null
               ? new FlowActionInfo(action)
               : null;
    }
}