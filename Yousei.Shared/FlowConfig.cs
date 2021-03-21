using System;
using System.Collections.Generic;

namespace Yousei.Shared
{
    public class FlowConfig
    {
        public List<BlockConfig> Actions { get; init; } = new();

        public BlockConfig Trigger { get; init; }
    }
}