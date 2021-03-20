using Yousei.Core;
using Microsoft.Extensions.Logging;

namespace YouseiRelaoded.Internal.Connectors.Log
{
    internal class LogConnection : SimpleConnection<LogConnection>
    {
        public LogConnection(ILogger logger)
        {
            AddAction("write", new WriteAction(logger));
        }
    }
}