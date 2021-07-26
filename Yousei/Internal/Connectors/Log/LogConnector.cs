using System.Reactive;
using Yousei.Core;
using Yousei.Shared;
using Microsoft.Extensions.Logging;

namespace YouseiRelaoded.Internal.Connectors.Log
{
    internal class LogConnector : SingletonConnector
    {
        public LogConnector(ILogger logger)
        {
            AddAction(new WriteAction(logger));
        }

        public override string Name { get; } = "log";
    }
}