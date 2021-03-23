using System.Reactive;
using Yousei.Core;
using Yousei.Shared;
using Microsoft.Extensions.Logging;

namespace YouseiRelaoded.Internal.Connectors.Log
{
    internal class LogConnector : Connector<Unit>
    {
        private readonly LogConnection connection;

        public LogConnector(ILogger<LogConnector> logger) : base("log")
        {
            connection = new LogConnection(logger);
        }

        protected override IConnection GetConnection(Unit configuration) => connection;
    }
}