using System.Collections.Generic;

namespace YouseiReloaded
{
    internal class FlowConfig
    {
        public IReadOnlyList<BlockConfig> Actions { get; }

        public BlockConfig Trigger { get; }
    }
}