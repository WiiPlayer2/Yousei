using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using Yousei.Core.Serialization.Yaml;
using Yousei.Serialization.Yaml;
using Yousei.Shared;

namespace Yousei.Internal.Database
{
    public class InMemoryDatabase : IConfigurationDatabase
    {
        private readonly Dictionary<string, Dictionary<string, (object? Config, SourceConfig Source)>> configs = new();

        private readonly IDeserializer deserializer;

        private readonly Dictionary<string, (FlowConfig? Flow, SourceConfig Source)> flows = new();

        private readonly IConfigurationProviderNotifier notifier;

        public InMemoryDatabase(IConfigurationProviderNotifier notifier)
        {
            this.notifier = notifier;
            deserializer = YamlUtil.BuildDeserializer();
        }

        public bool IsReadOnly { get; } = false;

        public Task<object?> GetConfiguration(string connector, string name)
            => Task.FromResult(GetConfigurationTuple(connector, name).Data);

        public Task<SourceConfig?> GetConfigurationSource(string connector, string name)
            => Task.FromResult(GetConfigurationTuple(connector, name).Source);

        public Task<FlowConfig?> GetFlow(string name)
            => Task.FromResult(GetFlowTuple(name).Flow);

        public Task<SourceConfig?> GetFlowSource(string name)
            => Task.FromResult(GetFlowTuple(name).Source);

        public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> ListConfigurations()
            => Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(
                configs.ToDictionary(o => o.Key, o => (IReadOnlyList<string>)new List<string>(o.Value.Keys)));

        public Task<IReadOnlyList<string>> ListFlows()
            => Task.FromResult<IReadOnlyList<string>>(flows.Keys.ToList());

        public Task SetConfiguration(string connector, string name, SourceConfig? source)
        {
            if (configs.TryGetValue(connector, out var connections))
            {
                if (source is null)
                {
                    connections.Remove(name);
                }
                else
                {
                    deserializer.TryDeserialize<object>(source.Content, out var configuration);
                    connections[name] = (configuration, source);
                }
            }
            else if (source is not null)
            {
                connections = new();
                configs.Add(connector, connections);
                deserializer.TryDeserialize<object>(source.Content, out var configuration);
                connections[name] = (configuration, source);
            }
            return Task.CompletedTask;
        }

        public Task SetFlow(string name, SourceConfig? source)
        {
            var flowConfig = default(FlowConfig);
            if (source is null)
            {
                flows.Remove(name);
            }
            else
            {
                deserializer.TryDeserialize(source.Content, out flowConfig);
                flows[name] = (flowConfig, source);
            }

            notifier.Flows.OnNext((name, flowConfig));
            return Task.CompletedTask;
        }

        private (object? Data, SourceConfig? Source) GetConfigurationTuple(string connector, string name)
        {
            if (!configs.TryGetValue(connector, out var connections)
                || !connections.TryGetValue(name, out var config))
                return default;
            return config;
        }

        private (FlowConfig? Flow, SourceConfig? Source) GetFlowTuple(string name)
        {
            if (!flows.TryGetValue(name, out var flow))
                return default;
            return flow;
        }
    }
}