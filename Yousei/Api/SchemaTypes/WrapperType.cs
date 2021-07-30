using HotChocolate.Configuration;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;

namespace Yousei.Api.SchemaTypes
{
    internal class WrapperType<TWrapped, TWrapper> : ObjectType<TWrapper>
        where TWrapper : Wrapper<TWrapped>
    {
        protected override void Configure(IObjectTypeDescriptor<TWrapper> descriptor)
        {
            descriptor
                .Field(o => o.Wrapped)
                .Ignore();
        }
    }
}