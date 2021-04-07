using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Http
{
    internal record RequestArguments
    {
        public IParameter Url { get; init; } = DefaultParameter.Instance;

        public IParameter Method { get; init; } = "GET".ToConstantParameter();

        public IParameter Body { get; init; } = string.Empty.ToConstantParameter();
    }
}