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
    internal class JsonStringType : ScalarType<Dummy<JToken, string>>
    {
        public JsonStringType() : base("JsonString", BindingBehavior.Explicit)
        {
        }

        public override bool IsInstanceOfType(IValueNode valueSyntax)
            => valueSyntax is StringValueNode;

        public override object? ParseLiteral(IValueNode valueSyntax, bool withDefaults = true)
        {
            if (valueSyntax is not StringValueNode stringNode)
                throw new InvalidOperationException();

            return (Dummy<JToken, string>)JToken.Parse(stringNode.Value);
        }

        public override IValueNode ParseResult(object? resultValue)
        {
            throw new NotImplementedException();
        }

        public override IValueNode ParseValue(object? runtimeValue)
        {
            throw new NotImplementedException();
        }

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            resultValue = default;
            if (runtimeValue is not Dummy<JToken, string> dummy)
                return false;

            resultValue = dummy.Value.ToString(Newtonsoft.Json.Formatting.None);
            return true;
        }
    }
}