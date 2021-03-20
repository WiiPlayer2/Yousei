using System;
using System.Collections.Generic;

namespace Yousei.Shared
{
    public class FlowConfig
    {
        public IReadOnlyList<BlockConfig> Actions { get; init; } = Array.Empty<BlockConfig>();

        public BlockConfig Trigger { get; init; }
    }
}