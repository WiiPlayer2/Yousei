using HotChocolate.Configuration;
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
    public class PropertyInfo : Wrapper<CLRPropertyInfo>
    {
        public PropertyInfo(CLRPropertyInfo wrapped) : base(wrapped)
        {
        }

        public bool IsParameter => Wrapped.PropertyType.IsAssignableTo(typeof(IParameter));

        public string Name => Wrapped.Name;

        public TypeInfo PropertyType => Wrapped.PropertyType;

        public static implicit operator PropertyInfo(CLRPropertyInfo propertyInfo)
            => new PropertyInfo(propertyInfo);
    }
}