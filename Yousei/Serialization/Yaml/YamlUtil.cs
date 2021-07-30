using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Yousei.Core;
using Yousei.Serialization.Yaml;

namespace Yousei.Serialization.Yaml
{
    public static class YamlUtil
    {
        public static IDeserializer BuildDeserializer()
            => new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .WithNodeDeserializer(new BlockConfigDeserializer())
                .WithNodeDeserializer(new ParameterDeserializer())
                .WithTagMapping("!v", typeof(VariableParameter))
                .WithTagMapping("!e", typeof(ExpressionParameter))
                .Build();

        public static bool TryDeserialize<T>(this IDeserializer deserializer, string input, out T? deserializedObject)
        {
            deserializedObject = default;
            try
            {
                deserializedObject = deserializer.Deserialize<T>(input);
                return true;
            }
            catch { }
            return false;
        }
    }
}