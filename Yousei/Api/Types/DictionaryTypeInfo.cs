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

    public class DictionaryTypeInfo : TypeInfo
    {
        public DictionaryTypeInfo(Type type) : base(type)
        {
        }

        public override TypeKind Kind { get; } = TypeKind.Dictionary;

        public TypeInfo GetKeyType()
        {
            var types = GetTypeHierarchy(Wrapped);
            if (types.Contains(typeof(IDictionary<,>)))
            {
                var dictionaryType = types.First(o => o.IsConstructedGenericType && o.GetGenericTypeDefinition() == typeof(IDictionary<,>));
                return dictionaryType.GenericTypeArguments[0];
            }
            else
            {
                return typeof(object);
            }
        }

        public TypeInfo GetValueType()
        {
            var types = GetTypeHierarchy(Wrapped);
            if (types.Contains(typeof(IDictionary<,>)))
            {
                var dictionaryType = types.First(o => o.IsConstructedGenericType && o.GetGenericTypeDefinition() == typeof(IDictionary<,>));
                return dictionaryType.GenericTypeArguments[1];
            }
            else
            {
                return typeof(object);
            }
        }
    }
}