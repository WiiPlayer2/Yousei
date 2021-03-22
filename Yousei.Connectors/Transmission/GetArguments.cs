using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    internal record GetArguments
    {
        public IParameter Fields { get; init; } = new ConstantParameter(System.Array.Empty<string>());
        public IParameter Ids { get; init; } = new ConstantParameter(System.Array.Empty<int>());
    }
}