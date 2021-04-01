using HotChocolate.Language;
using HotChocolate.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yousei.Api.SchemaType
{
    public class JsonType : ScalarType
    {
        public JsonType() : base("Json", BindingBehavior.Implicit)
        {
        }

        public override Type RuntimeType { get; } = typeof(JToken);

        public override bool IsInstanceOfType(IValueNode valueSyntax) => true;

        public override object ParseLiteral(IValueNode valueSyntax, bool withDefaults = true)
        {
            switch (valueSyntax)
            {
                case ObjectValueNode objectValueNode:
                    var jobject = new JObject();
                    foreach (var field in objectValueNode.Fields)
                    {
                        jobject.Add(field.Name.Value, ParseLiteral(field.Value) as JToken);
                    }
                    return jobject;

                case ListValueNode listValueNode:
                    return new JArray(listValueNode.Items.Select(value => ParseLiteral(value)).ToArray());

                case IntValueNode intValueNode:
                    return new JValue(intValueNode.ToInt64());

                default:
                    return new JValue(valueSyntax.Value);
            }
        }

        public override IValueNode ParseResult(object resultValue)
        {
            throw new NotImplementedException();
        }

        public override IValueNode ParseValue(object runtimeValue)
        {
            throw new NotImplementedException();
        }

        public override bool TryDeserialize(object resultValue, out object runtimeValue)
        {
            throw new NotImplementedException();
        }

        public override bool TrySerialize(object runtimeValue, out object resultValue)
        {
            resultValue = default;
            if (runtimeValue is null)
                resultValue = null;
            else if (runtimeValue is JObject jobject)
                resultValue = jobject.Properties().ToDictionary(o => o.Name, o =>
                {
                    TrySerialize(o.Value, out var result);
                    return result;
                });
            else if (runtimeValue is JArray jarray)
                resultValue = jarray.Select(o =>
                {
                    TrySerialize(o, out var result);
                    return result;
                }).ToList();
            else if (runtimeValue is JValue jvalue)
                resultValue = jvalue.Value;
            else
                return false;
            return true;
        }
    }
}