using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Data
{
    internal record ClearArguments
    {
        public IParameter Path { get; init; } = DefaultParameter.Instance;
    }
}