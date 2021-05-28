using System.Collections.Generic;
using Yousei.Shared;

namespace Yousei.Serialization.Yaml
{
    internal class YamlConfig
    {
        public Dictionary<string, Dictionary<string, object>> Connections { get; init; } = new();

        public Dictionary<string, FlowConfig> Flows { get; init; } = new();
    }
}