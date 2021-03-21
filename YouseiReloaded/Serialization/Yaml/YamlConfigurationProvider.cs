using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;
using YamlDotNet.Serialization;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Core;
using Yousei.Core;
using YamlDotNet.Core.Events;
using System.Reactive.Linq;

namespace YouseiReloaded.Serialization.Yaml
{
    internal class BlockConfigDeserializer : INodeDeserializer
    {
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (expectedType != typeof(BlockConfig))
            {
                value = default;
                return false;
            }

            var map = nestedObjectDeserializer(reader, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            var config = new BlockConfig();
            if (map.Remove("type", out var type))
                config.Type = (string)type;
            if (map.Remove("configuration", out var configuration))
                config.Configuration = (string)configuration;
            config.Arguments = map;
            value = config;
            return true;
        }
    }

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

    internal class YamlConfig
    {
        public Dictionary<string, Dictionary<string, object>> Connections { get; init; } = new();

        public Dictionary<string, FlowConfig> Flows { get; init; } = new();
    }

    internal class YamlConfigurationProvider : IConfigurationProvider
    {
        private readonly YamlConfig config;

        private readonly string path = @"D:\Data\Dropbox\Workspace\DotNET\Tools\Yousei\reloaded\demo_config.yaml";

        public YamlConfigurationProvider()
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .WithNodeDeserializer(new BlockConfigDeserializer())
                .WithNodeDeserializer(new ParameterDeserializer())
                .WithTagMapping("!v", typeof(VariableParameter))
                .WithTagMapping("!e", typeof(ExpressionParameter))
                .Build();
            using var reader = new StreamReader(path);
            config = deserializer.Deserialize<YamlConfig>(reader) ?? new YamlConfig();
        }

        public object GetConnectionConfiguration(string type, string name)
            => config.Connections.TryGetValue(type, out var configurations)
                && configurations.TryGetValue(name, out var configuration)
                ? configuration
                : default;

        public FlowConfig GetFlow(string name)
            => config.Flows.TryGetValue(name, out var flow)
                ? flow
                : default;

        public IObservable<(string Name, FlowConfig Config)> GetFlows()
            => config.Flows
                .Select(o => (o.Key, o.Value))
                .ToObservable();
    }
}