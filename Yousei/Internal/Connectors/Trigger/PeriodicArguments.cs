using System;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Trigger
{
    internal record PeriodicArguments
    {
        public BlockConfig? Action { get; init; }

        public string? Path { get; init; }

        public TimeSpan Interval { get; init; } = TimeSpan.FromMinutes(15);
    }
}