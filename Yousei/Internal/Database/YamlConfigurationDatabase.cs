using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using Yousei.Serialization.Yaml;
using Yousei.Shared;

namespace Yousei.Internal.Database
{
    internal class YamlConfigurationDatabase : IConfigurationDatabase
    {
        private const string FILE_EXTENSION = "yaml";

        private const string LANGUAGE = "yaml";

        private readonly string connectionsPath;

        private readonly IDeserializer deserializer;

        private readonly string flowsPath;

        private readonly IConfigurationProviderNotifier notifier;

        public YamlConfigurationDatabase(IConfiguration configuration, IConfigurationProviderNotifier notifier)
        {
            var options = configuration.GetSection(Options.KEY).Get<Options>();
            connectionsPath = Path.Combine(options.Path, "connections");
            flowsPath = Path.Combine(options.Path, "flows");
            Directory.CreateDirectory(connectionsPath);
            Directory.CreateDirectory(flowsPath);
            deserializer = YamlUtil.BuildDeserializer();
            this.notifier = notifier;
        }

        public bool IsReadOnly { get; } = false;

        public async Task<object?> GetConfiguration(string connector, string name)
            => TryDeserializeSource<object>(await GetConfigurationSource(connector, name));

        public Task<SourceConfig?> GetConfigurationSource(string connector, string name)
            => TryGetSource(GetConfigurationPath(connector, name));

        public async Task<FlowConfig?> GetFlow(string name)
            => TryDeserializeSource<FlowConfig>(await GetFlowSource(name));

        public Task<SourceConfig?> GetFlowSource(string name)
            => TryGetSource(GetFlowPath(name));

        public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> ListConfigurations()
        {
            var configurations = Directory.EnumerateDirectories(connectionsPath)
                .Select(directory =>
                {
                    var files = Directory.EnumerateFiles(directory, $"*.{FILE_EXTENSION}")
                        .Select(file => Path.GetFileNameWithoutExtension(file))
                        .ToList();
                    return (Connector: Path.GetFileName(directory), Items: (IReadOnlyList<string>)files);
                })
                .ToDictionary(o => o.Connector, o => o.Items);
            return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(configurations);
        }

        public Task<IReadOnlyList<string>> ListFlows()
        {
            var files = Directory.EnumerateFiles(flowsPath, $"*.{FILE_EXTENSION}")
                .Select(file => Path.GetFileNameWithoutExtension(file))
                .ToList();
            return Task.FromResult<IReadOnlyList<string>>(files);
        }

        public async Task SetConfiguration(string connector, string name, SourceConfig? source)
        {
            var path = GetConfigurationPath(connector, name);
            if (source is null)
            {
                File.Delete(path);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
                await File.WriteAllTextAsync(path, source.Content);
            }
        }

        public async Task SetFlow(string name, SourceConfig? source)
        {
            var path = GetFlowPath(name);
            if (source is null)
                File.Delete(path);
            else
                await File.WriteAllTextAsync(path, source.Content);
            var flow = TryDeserializeSource<FlowConfig>(source);
            notifier.Flows.OnNext((name, flow));
        }

        private string GetConfigurationPath(string connector, string name)
            => Path.Combine(connectionsPath, connector, $"{name}.{FILE_EXTENSION}");

        private string GetFlowPath(string name)
            => Path.Combine(flowsPath, $"{name}.{FILE_EXTENSION}");

        private T? TryDeserializeSource<T>(SourceConfig? source)
        {
            if (source is null)
                return default;

            try
            {
                return deserializer.Deserialize<T>(source.Content);
            }
            catch
            {
                return default;
            }
        }

        private async Task<SourceConfig?> TryGetSource(string path)
        {
            if (!File.Exists(path))
                return default;
            return new SourceConfig(LANGUAGE, await File.ReadAllTextAsync(path));
        }

        private class Options
        {
            public const string KEY = "YamlConfigurationDatabase";

            public string Path { get; init; } = string.Empty;
        }
    }
}