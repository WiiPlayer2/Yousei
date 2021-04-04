using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Database
{
    internal class ConfigurationProviderDatabase : IConfigurationDatabase
    {
        public ConfigurationProviderDatabase(IConfigurationProvider configurationProvider)
        {
            ConfigurationProvider = configurationProvider;
        }

        public IConfigurationProvider ConfigurationProvider { get; }

        public Task<bool> IsReadOnly { get; } = Task.FromResult(true);

        public Task<object?> GetConfiguration(string connector, string name)
            => Task.FromResult(ConfigurationProvider.GetConnectionConfiguration(connector, name));

        public async Task<SourceConfig?> GetConfigurationSource(string connector, string name)
            => new SourceConfig("json", (await GetConfiguration(connector, name)).Map<JToken>()?.ToString() ?? string.Empty);

        public Task<FlowConfig?> GetFlow(string name)
            => Task.FromResult(ConfigurationProvider.GetFlow(name));

        public async Task<SourceConfig?> GetFlowSource(string name)
            => new SourceConfig("json", (await GetFlow(name)).Map<JToken>()?.ToString() ?? string.Empty);

        public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> ListConfigurations()
            => await ConfigurationProvider.GetConnectionConfigurations()
                .ToLookup(o => o.Connector, o => o.Name)
                .Select(lookup => lookup.ToDictionary(o => o.Key, o => (IReadOnlyList<string>)o.ToList()));

        public async Task<IReadOnlyList<string>> ListFlows()
        {
            var flowTuples = await ConfigurationProvider.GetFlows().ToList();
            return new List<string>(flowTuples.Select(item => item.Name));
        }

        public Task SetConfiguration(string connector, string name, SourceConfig? source)
            => throw new NotImplementedException();

        public Task SetFlow(string name, SourceConfig? source)
            => throw new NotImplementedException();
    }
}