using System.Collections.Generic;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal class IfArguments
    {
        public List<BlockConfig> Else { get; init; }

        public IParameter If { get; init; }

        public List<BlockConfig> Then { get; init; }
    }
}