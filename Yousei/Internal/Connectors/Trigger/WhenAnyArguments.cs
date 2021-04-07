using System.Collections.Generic;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Trigger
{
    internal record WhenAnyArguments
    {
        public List<BlockConfig> Triggers { get; init; } = new();
    }
}