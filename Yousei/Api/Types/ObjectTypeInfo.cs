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

    public class ObjectTypeInfo : TypeInfo
    {
        public ObjectTypeInfo(Type type) : base(type)
        {
        }

        public override TypeKind Kind { get; } = TypeKind.Object;

        public IEnumerable<PropertyInfo> GetProperties() => Wrapped.GetProperties().Select(o => (PropertyInfo)o);
    }
}