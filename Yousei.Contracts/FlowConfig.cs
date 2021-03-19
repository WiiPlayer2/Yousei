using System.Collections.Generic;

namespace Yousei.Contracts
{
    public class FlowConfig
    {
        public IReadOnlyList<BlockConfig> Actions { get; }

        public BlockConfig Trigger { get; }
    }
}