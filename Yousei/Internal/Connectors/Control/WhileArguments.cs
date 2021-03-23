using System.Collections.Generic;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal record WhileArguments
    {
        public IParameter Condition { get; init; }
        public List<BlockConfig> Actions { get; init; }
    }
}