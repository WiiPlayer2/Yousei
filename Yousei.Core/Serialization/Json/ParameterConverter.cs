using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Core.Serialization.Json
{
    internal class ParameterConverter : JsonConverter
    {
        internal enum ParameterType
        {
            Constant,

            Variable,

            Expression,
        }

        private record Dto(
            ParameterType? ___ParameterType,
            JToken Config);

        public override bool CanConvert(Type objectType)
            => objectType.IsAssignableTo(typeof(IParameter));

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var valueType = objectType.GetValueType();
            var jtoken = JToken.ReadFrom(reader);
            if (!jtoken.TryToObject<Dto>(out var dto) || dto is null || dto.___ParameterType is null)
            {
                return new ConstantParameter(jtoken).Map(valueType);
            }

            var parameter = dto.___ParameterType.Value.Match<IParameter>(
                () => new ConstantParameter(dto.Config).Map(valueType),
                () => new VariableParameter(dto.Config.ToObject<string>() ?? string.Empty).Map(valueType),
                () => new ExpressionParameter(dto.Config.ToObject<string>() ?? string.Empty).Map(valueType));
            return parameter;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var dto = value switch
            {
                ConstantParameter constantParameter => new Dto(ParameterType.Constant, constantParameter.Value.Map<JToken>() ?? JValue.CreateNull()),
                VariableParameter variableParameter => new Dto(ParameterType.Variable, variableParameter.Path),
                ExpressionParameter expressionParameter => new Dto(ParameterType.Expression, expressionParameter.Code),
                _ => throw new NotImplementedException(),
            };
            serializer.Serialize(writer, dto);
        }
    }
}