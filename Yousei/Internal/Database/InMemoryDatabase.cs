using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Internal.Database
{
    public class InMemoryDatabase : IConfigurationDatabase
    {
        private readonly Dictionary<string, Dictionary<string, object>> configs = new();

        private readonly Dictionary<string, FlowConfig> flows = new();

        private readonly IConfigurationProviderNotifier notifier;

        public InMemoryDatabase(IConfigurationProviderNotifier notifier)
        {
            this.notifier = notifier;
        }

        public Task<bool> IsReadOnly { get; } = Task.FromResult(false);

        public Task<object> GetConfiguration(string connector, string name)
        {
            if (!configs.TryGetValue(connector, out var connections)
                || !connections.TryGetValue(name, out var config))
                return Task.FromResult<object>(default);
            return Task.FromResult(config);
        }

        public Task<FlowConfig> GetFlow(string name)
        {
            if (!flows.TryGetValue(name, out var flow))
                return Task.FromResult<FlowConfig>(default);
            return Task.FromResult(flow);
        }

        public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> ListConfigurations()
            => Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(
                configs.ToDictionary(o => o.Key, o => (IReadOnlyList<string>)new List<string>(o.Value.Keys)));

        public Task<IReadOnlyList<string>> ListFlows()
            => Task.FromResult<IReadOnlyList<string>>(flows.Keys.ToList());

        public Task SetConfiguration(string connector, string name, object configuration)
        {
            if (configs.TryGetValue(connector, out var connections))
            {
                if (configuration is null)
                    connections.Remove(name);
                else
                    connections[name] = configuration;
            }
            else if (configuration is not null)
            {
                connections = new Dictionary<string, object>();
                configs.Add(connector, connections);
                connections[name] = configuration;
            }
            return Task.CompletedTask;
        }

        public Task SetFlow(string name, FlowConfig flowConfig)
        {
            if (flowConfig is null)
                flows.Remove(name);
            else
                flows[name] = flowConfig;
            notifier.Flows.OnNext((name, flowConfig));
            return Task.CompletedTask;
        }
    }
}