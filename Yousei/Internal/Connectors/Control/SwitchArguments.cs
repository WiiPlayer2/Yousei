using System.Collections.Generic;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal class SwitchArguments
    {
        public List<(object Case, List<BlockConfig> Actions)> Cases { get; init; } = new();

        public List<BlockConfig> Default { get; init; } = new List<BlockConfig>();

        public IParameter Value { get; init; } = DefaultParameter.Instance;
    }
}