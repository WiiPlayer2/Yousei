using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IConfigurationDatabase
    {
        Task<bool> IsReadOnly { get; }

        Task<object> GetConfiguration(string connector, string name);

        Task<FlowConfig> GetFlow(string name);

        Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> ListConfigurations();

        Task<IReadOnlyList<string>> ListFlows();

        Task SetConfiguration(string connector, string name, object configuration);

        Task SetFlow(string name, FlowConfig flowConfig);
    }
}