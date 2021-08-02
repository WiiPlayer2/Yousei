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
    internal class JsonObjectType : ScalarType
    {
        public JsonObjectType() : base("JsonObject", BindingBehavior.Implicit)
        {
        }

        public override Type RuntimeType { get; } = typeof(Dummy<JToken, object>);

        public override bool IsInstanceOfType(IValueNode valueSyntax) => true;

        public override object ParseLiteral(IValueNode valueSyntax, bool withDefaults = true)
        {
            switch (valueSyntax)
            {
                case ObjectValueNode objectValueNode:
                    var jobject = new JObject();
                    foreach (var field in objectValueNode.Fields)
                    {
                        jobject.Add(field.Name.Value, (ParseLiteral(field.Value) as Dummy<JToken, object>)!.Value);
                    }
                    return (Dummy<JToken, object>)jobject;

                case ListValueNode listValueNode:
                    return (Dummy<JToken, object>)new JArray(listValueNode.Items.Select(value => (ParseLiteral(value) as Dummy<JToken, object>)!.Value).ToArray());

                case IntValueNode intValueNode:
                    return (Dummy<JToken, object>)new JValue(intValueNode.ToInt64());

                default:
                    return (Dummy<JToken, object>)new JValue(valueSyntax.Value);
            }
        }

        public override IValueNode ParseResult(object? resultValue)
        {
            throw new NotImplementedException();
        }

        public override IValueNode ParseValue(object? runtimeValue)
        {
            throw new NotImplementedException();
        }

        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            throw new NotImplementedException();
        }

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            resultValue = default;
            if (runtimeValue is null || runtimeValue is not Dummy<JToken, object> dummy)
                return false;

            resultValue = Serialize(dummy.Value);
            return true;

            object? Serialize(JToken jtoken)
                => jtoken switch
                {
                    JObject jobject => jobject.Properties().ToDictionary(o => o.Name, o => Serialize(o.Value)),
                    JArray jarray => jarray.Select(Serialize).ToList(),
                    JValue jvalue => jvalue.Value,
                    _ => throw new NotImplementedException(),
                };
        }
    }
}