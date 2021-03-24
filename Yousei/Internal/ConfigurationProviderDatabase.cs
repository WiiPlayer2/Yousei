using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Internal
{
    internal class ConfigurationProviderDatabase : IConfigurationDatabase
    {
        public bool IsReadOnly { get; } = false;

        public IConfigurationProvider ConfigurationProvider { get; }

        public ConfigurationProviderDatabase(IConfigurationProvider configurationProvider)
        {
            ConfigurationProvider = configurationProvider;
        }

        public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> ListConfigurations()
            => await ConfigurationProvider.ListConnectionConfigurations()
                .ToLookup(o => o.Connector, o => o.Name)
                .Select(lookup => lookup.ToDictionary(o => o.Key, o => (IReadOnlyList<string>)o.ToList()));

        public async Task<IReadOnlyList<string>> ListFlows()
        {
            var flowTuples = await ConfigurationProvider.GetFlows().ToList();
            return new List<string>(flowTuples.Select(item => item.Name));
        }

        public Task<object> GetConfiguration(string connector, string name)
            => Task.FromResult(ConfigurationProvider.GetConnectionConfiguration(connector, name));

        public Task<FlowConfig> GetFlow(string name)
            => Task.FromResult(ConfigurationProvider.GetFlow(name));

        public Task SetConfiguration(string connector, string name, object configuration)
            => throw new NotImplementedException();

        public Task SetFlow(string name, FlowConfig flowConfig)
            => throw new NotImplementedException();
    }
}