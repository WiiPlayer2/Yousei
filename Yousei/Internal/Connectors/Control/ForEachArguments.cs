using System.Collections;
using System.Collections.Generic;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Control
{
    internal record ForEachArguments
    {
        public List<BlockConfig> Actions { get; init; } = new();

        public IParameter<ArrayList> Collection { get; init; } = DefaultParameter<ArrayList>.Instance;
    }
}