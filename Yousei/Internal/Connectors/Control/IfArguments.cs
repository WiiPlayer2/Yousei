using System.Collections.Generic;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Control
{
    internal class IfArguments
    {
        public List<BlockConfig> Else { get; init; } = new();

        public IParameter<bool> If { get; init; } = DefaultParameter<bool>.Instance;

        public List<BlockConfig> Then { get; init; } = new();
    }
}