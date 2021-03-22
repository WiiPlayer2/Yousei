using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    internal record RemoveArguments
    {
        public IParameter Ids { get; init; } = new ConstantParameter(System.Array.Empty<int>());
        public IParameter DeleteData { get; init; } = new ConstantParameter(false);
    }
}