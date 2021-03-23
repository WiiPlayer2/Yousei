using Yousei.Core;
using Microsoft.Extensions.Logging;

namespace YouseiRelaoded.Internal.Connectors.Log
{
    internal class LogConnection : SimpleConnection
    {
        public LogConnection(ILogger logger)
        {
            AddAction("write", new WriteAction(logger));
        }
    }
}