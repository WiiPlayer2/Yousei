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

    public class TypeInfoType : InterfaceType<TypeInfo>
    {
        protected override void Configure(IInterfaceTypeDescriptor<TypeInfo> descriptor)
        {
            descriptor
                .Field(o => o.Wrapped)
                .Ignore();
        }
    }
}