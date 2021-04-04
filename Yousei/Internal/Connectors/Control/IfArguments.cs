using System.Collections.Generic;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal class IfArguments
    {
        public List<BlockConfig> Else { get; init; } = new();

        public IParameter If { get; init; } = DefaultParameter.Instance;

        public List<BlockConfig> Then { get; init; } = new();
    }
}