using System.Collections.Generic;

namespace YouseiReloaded
{
    internal class BlockConfig
    {
        public IReadOnlyDictionary<string, object> Arguments { get; set; }

        public string ConfigurationName { get; set; } = "default";

        public string Type { get; set; }
    }
}