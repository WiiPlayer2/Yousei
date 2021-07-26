﻿using HotChocolate.Configuration;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;
using CLRPropertyInfo = System.Reflection.PropertyInfo;

namespace Yousei.Api.Types
{
    public class TypeInfo : Wrapper<Type>
    {
        public TypeInfo(Type type) : base(type)
        {
        }

        public string? FullName => Wrapped.FullName;

        public static implicit operator TypeInfo(Type type)
            => new TypeInfo(type);

        public IEnumerable<PropertyInfo> GetProperties() => Wrapped.GetProperties().Select(o => (PropertyInfo)o);
    }
}