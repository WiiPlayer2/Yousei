using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Http
{
    internal record RequestArguments
    {
        public IParameter<string> Url { get; init; } = DefaultParameter<string>.Instance;

        public IParameter<string> Method { get; init; } = "GET".ToConstantParameter();

        public IParameter<string> Body { get; init; } = string.Empty.ToConstantParameter();
    }
}