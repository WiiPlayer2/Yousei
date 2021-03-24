﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Serialization.Json
{
    internal class ParameterConverter : JsonConverter
    {
        internal enum ParameterType
        {
            Constant,

            Variable,

            Expression,
        }

        private record Dto(ParameterType Type, JToken Config);

        public override bool CanConvert(Type objectType)
            => objectType.IsAssignableTo(typeof(IParameter));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jtoken = JToken.ReadFrom(reader);
            if (!jtoken.TryToObject<Dto>(out var dto))
            {
                return new ConstantParameter(jtoken);
            }

            var parameter = dto.Type.Match<IParameter>(
                () => new ConstantParameter(dto.Config),
                () => new VariableParameter(dto.Config.ToObject<string>()),
                () => new ExpressionParameter(dto.Config.ToObject<string>()));
            return parameter;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dto = value switch
            {
                ConstantParameter constantParameter => new Dto(ParameterType.Constant, constantParameter.Value.Map<JToken>()),
                VariableParameter variableParameter => new Dto(ParameterType.Variable, variableParameter.Path),
                ExpressionParameter expressionParameter => new Dto(ParameterType.Expression, expressionParameter.Code),
                _ => throw new NotImplementedException(),
            };
            serializer.Serialize(writer, dto);
        }
    }
}