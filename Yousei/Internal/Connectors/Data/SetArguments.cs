using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Data
{
    internal record SetArguments
    {
        public IParameter Path { get; init; } = DefaultParameter.Instance;

        public IParameter Value { get; init; } = DefaultParameter.Instance;
    }
}