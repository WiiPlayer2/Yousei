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
    public abstract class TypeInfo : Wrapper<Type>
    {
        public TypeInfo(Type type) : base(type)
        {
        }

        public string? FullName => Wrapped.FullName;

        public abstract TypeKind Kind { get; }

        public static TypeKind GetKind(Type type)
        {
            if (type.IsPrimitive || type == typeof(string))
                return TypeKind.Scalar;

            var types = GetTypeHierarchy(type);

            if (types.Contains(typeof(IParameter)))
                return GetKind(type.GetValueType());

            if (types.Contains(typeof(IDictionary)) || types.Contains(typeof(IDictionary<,>)))
                return TypeKind.Dictionary;

            if (type.IsArray || types.Contains(typeof(IEnumerable)))
                return TypeKind.List;

            if (type == typeof(object))
                return TypeKind.Any;

            if (type == typeof(Unit))
                return TypeKind.Unit;

            return TypeKind.Object;
        }

        public static IReadOnlyList<Type> GetTypeHierarchy(Type type)
        {
            var types = new[] { type }.Concat(type.GetInterfaces()).Concat(GetBaseTypes(type)).ToList();
            types = types.Concat(types.Where(o => o.IsGenericType).Select(o => o.GetGenericTypeDefinition())).ToList();
            return types;

            IEnumerable<Type> GetBaseTypes(Type type)
                => type.BaseType is not null
                    ? new[] { type.BaseType }.Concat(GetBaseTypes(type.BaseType))
                    : Enumerable.Empty<Type>();
        }

        public static implicit operator TypeInfo(Type type)
            => GetKind(type) switch
            {
                TypeKind.Scalar => new ScalarTypeInfo(type),
                TypeKind.Object => new ObjectTypeInfo(type),
                TypeKind.List => new ListTypeInfo(type),
                TypeKind.Dictionary => new DictionaryTypeInfo(type),
                TypeKind.Unit => new UnitTypeInfo(type),
                TypeKind.Any => new AnyTypeInfo(type),
                _ => throw new NotImplementedException(),
            };
    }
}