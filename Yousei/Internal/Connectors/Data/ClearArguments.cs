using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Data
{
    internal record ClearArguments
    {
        public IParameter<string> Path { get; init; } = DefaultParameter<string>.Instance;
    }
}