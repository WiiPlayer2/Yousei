using Yousei.Core;
using Yousei.Shared;
using Microsoft.Extensions.Logging;

namespace YouseiRelaoded.Internal.Connectors.Log
{
    internal record WriteArguments
    {
        public IParameter Level { get; init; } = new ConstantParameter(LogLevel.Information);

        public IParameter Tag { get; init; } = new ConstantParameter("logging");

        public IParameter Message { get; init; } = DefaultParameter.Instance;
    }
}