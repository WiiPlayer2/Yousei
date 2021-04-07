﻿using System.Collections.Generic;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal record WhileArguments
    {
        public IParameter Condition { get; init; } = DefaultParameter.Instance;

        public List<BlockConfig> Actions { get; init; } = new();
    }
}