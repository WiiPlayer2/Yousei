using System.Collections.Generic;

namespace Yousei.Shared
{
    public class BlockConfig
    {
        public object? Arguments { get; set; }

        public string Configuration { get; set; } = "default";

        public string Type { get; set; } = string.Empty;
    }
}