using Yousei.Core;
using Yousei.Shared;
using Microsoft.Extensions.Logging;

namespace YouseiRelaoded.Internal.Connectors.Log
{
    internal record WriteArguments
    {
        public IParameter<LogLevel> Level { get; init; } = LogLevel.Information.ToConstantParameter();

        public IParameter<string> Tag { get; init; } = "logging".ToConstantParameter();

        public IParameter Message { get; init; } = DefaultParameter.Instance;
    }
}