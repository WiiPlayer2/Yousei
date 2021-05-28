﻿using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Internal
{
    internal record SendValueArguments
    {
        public IParameter Topic { get; init; } = DefaultParameter.Instance;

        public IParameter Value { get; init; } = DefaultParameter.Instance;
    }
}