using HotChocolate.Language;
using HotChocolate.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Api.Types;

namespace Yousei.Api.SchemaTypes
{
    internal class JsonType : ObjectType<JToken>
    {
        protected override void Configure(IObjectTypeDescriptor<JToken> descriptor)
        {
            descriptor.Name("Json");
            descriptor.BindFieldsExplicitly();
            descriptor
                .Field("object")
                .Type<JsonObjectType>()
                .Resolve(ctx => (Dummy<JToken, object>)ctx.Parent<JToken>());
            descriptor
                .Field("string")
                .Type<JsonStringType>()
                .Resolve(ctx => (Dummy<JToken, string>)ctx.Parent<JToken>());
        }
    }
}