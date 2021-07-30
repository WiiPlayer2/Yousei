using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Data
{
    internal record SetArguments
    {
        public IParameter<string> Path { get; init; } = DefaultParameter<string>.Instance;

        public IParameter Value { get; init; } = DefaultParameter.Instance;
    }
}