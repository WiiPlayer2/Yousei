using System;
using YamlDotNet.Serialization;
using YamlDotNet.Core;
using Yousei.Core;
using YamlDotNet.Core.Events;

namespace Yousei.Core.Serialization.Yaml
{
    internal class ParameterDeserializer : INodeDeserializer
    {
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            value = default;
            if (expectedType != typeof(VariableParameter) && expectedType != typeof(ExpressionParameter))
                return false;

            var argument = reader.Consume<Scalar>();
            if (expectedType == typeof(VariableParameter))
                value = new VariableParameter(argument.Value);
            else if (expectedType == typeof(ExpressionParameter))
                value = new ExpressionParameter(argument.Value);
            else
                return false;
            return true;
        }
    }
}