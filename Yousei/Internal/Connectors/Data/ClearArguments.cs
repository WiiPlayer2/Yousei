using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Data
{
    internal record ClearArguments
    {
        public IParameter Path { get; init; } = DefaultParameter.Instance;
    }
}