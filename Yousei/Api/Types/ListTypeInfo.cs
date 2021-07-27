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

    public class ListTypeInfo : TypeInfo
    {
        public ListTypeInfo(Type listType) : base(listType)
        {
        }

        public override TypeKind Kind { get; } = TypeKind.List;

        public TypeInfo GetItemType()
        {
            if (Wrapped.IsArray)
                return Wrapped.GetElementType() ?? throw new InvalidOperationException();

            var types = GetTypeHierarchy(Wrapped);
            if (types.Contains(typeof(IEnumerable<>)))
                return types
                    .First(o => o.IsConstructedGenericType && o.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .GenericTypeArguments[0];

            return typeof(object);
        }
    }
}