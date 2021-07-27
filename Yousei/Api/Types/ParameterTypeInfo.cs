using HotChocolate.Configuration;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors.Definitions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;

namespace Yousei.Api.Types
{

    public class ParameterTypeInfo : TypeInfo
    {
        public ParameterTypeInfo(Type type) : base(type)
        {
        }

        public override TypeKind Kind { get; } = TypeKind.Parameter;

        public TypeInfo GetValueType() => typeof(object);
    }
}