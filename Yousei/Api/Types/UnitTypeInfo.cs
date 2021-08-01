using HotChocolate.Configuration;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors.Definitions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Api.Types
{

    internal class UnitTypeInfo : TypeInfo
    {
        public UnitTypeInfo(Type type) : base(type)
        {
        }

        public override TypeKind Kind { get; } = TypeKind.Unit;
    }
}