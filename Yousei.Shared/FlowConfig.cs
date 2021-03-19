using System.Collections.Generic;

namespace Yousei.Shared
{
    public class FlowConfig
    {
        public IReadOnlyList<BlockConfig> Actions { get; }

        public BlockConfig Trigger { get; }
    }
}