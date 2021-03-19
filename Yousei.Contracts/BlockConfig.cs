using System.Collections.Generic;

namespace Yousei.Contracts
{
    public class BlockConfig
    {
        public IReadOnlyDictionary<string, object> Arguments { get; set; }

        public string ConfigurationName { get; set; } = "default";

        public string Type { get; set; }
    }
}