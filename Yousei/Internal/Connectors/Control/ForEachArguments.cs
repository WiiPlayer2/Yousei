using System.Collections.Generic;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal record ForEachArguments
    {
        public List<BlockConfig> Actions { get; init; }

        public IParameter Collection { get; init; }
    }
}