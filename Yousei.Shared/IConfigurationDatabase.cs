using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IConfigurationDatabase
    {
        bool IsReadOnly { get; }

        Task<object?> GetConfiguration(string connector, string name);

        Task<SourceConfig?> GetConfigurationSource(string connector, string name);

        Task<FlowConfig?> GetFlow(string name);

        Task<SourceConfig?> GetFlowSource(string name);

        Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> ListConfigurations();

        Task<IReadOnlyList<string>> ListFlows();

        Task SetConfiguration(string connector, string name, SourceConfig? source);

        Task SetFlow(string name, SourceConfig? source);
    }
}