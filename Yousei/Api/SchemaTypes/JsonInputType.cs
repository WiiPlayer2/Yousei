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
    internal class JsonInputType : InputObjectType<JToken>
    {
        public override object? ParseLiteral(IValueNode valueSyntax, bool withDefaults = true)
        {
            if (valueSyntax is not ObjectValueNode objectNode)
                throw new InvalidOperationException();

            if (objectNode.Fields.Count != 1)
                throw new ArgumentException("Exactly one field has to be set.");

            var field = objectNode.Fields[0];
            return field.Name.Value switch
            {
                "object" => (JToken)(Dummy<JToken, object>)new JsonObjectType().ParseLiteral(field.Value),
                _ => throw new NotImplementedException(),
            };
        }

        protected override void Configure(IInputObjectTypeDescriptor<JToken> descriptor)
        {
            descriptor.Name("JsonInput");
            descriptor.BindFieldsExplicitly();
            descriptor
                .Field("object")
                .Type<JsonObjectType>()
                .DefaultValue(NullValueNode.Default);
        }
    }
}