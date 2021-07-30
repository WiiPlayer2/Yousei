using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Internal
{
    internal record SendValueArguments
    {
        public IParameter<string> Topic { get; init; } = DefaultParameter<string>.Instance;

        public IParameter Value { get; init; } = DefaultParameter.Instance;
    }
}