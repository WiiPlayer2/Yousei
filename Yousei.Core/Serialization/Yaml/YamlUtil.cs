using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Yousei.Core.Serialization.Yaml
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
    }
}