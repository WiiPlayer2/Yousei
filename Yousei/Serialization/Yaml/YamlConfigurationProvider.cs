using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Yousei.Core;
using Yousei.Shared;
using IConfigurationProvider = Yousei.Shared.IConfigurationProvider;

namespace YouseiReloaded.Serialization.Yaml
{
    internal class YamlConfigurationProvider : IConfigurationProvider
    {
        private readonly YamlConfig config;

        public YamlConfigurationProvider(IConfiguration configuration)
        {
            var options = configuration.GetSection(Options.KEY).Get<Options>();
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .WithNodeDeserializer(new BlockConfigDeserializer())
                .WithNodeDeserializer(new ParameterDeserializer())
                .WithTagMapping("!v", typeof(VariableParameter))
                .WithTagMapping("!e", typeof(ExpressionParameter))
                .Build();
            using var reader = new StreamReader(options.File);
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

        public IObservable<(string, string, object)> ListConnectionConfigurations()
        {
            return config.Connections
                .SelectMany(o => o.Value, (name, conn) => (name.Key, conn.Key, conn.Value))
                .ToObservable();
        }
    }
}