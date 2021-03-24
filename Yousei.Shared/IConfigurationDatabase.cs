using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IConfigurationDatabase
    {
        bool IsReadOnly { get; }

        Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetConfigurations();

        Task<object> GetConfiguration(string connector, string name);

        Task SetConfiguration(string connector, string name, object configuration);

        Task<IReadOnlyList<string>> GetFlows();

        Task<FlowConfig> GetFlow(string name);

        Task SetFlow(string name, FlowConfig flowConfig);
    }
}