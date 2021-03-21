using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;
using YamlDotNet.Serialization;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;
using Yousei.Core;
using System.Reactive.Linq;

namespace YouseiReloaded.Serialization.Yaml
{
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