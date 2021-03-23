using System.Reactive;
using Yousei.Core;
using Yousei.Shared;
using Microsoft.Extensions.Logging;

namespace YouseiRelaoded.Internal.Connectors.Log
{
    internal class LogConnector : SingletonConnector
    {
        private readonly ILogger<LogConnector> logger;

        public LogConnector(ILogger<LogConnector> logger) : base("log")
        {
            this.logger = logger;
        }

        protected override IConnection CreateConnection() => new LogConnection(logger);
    }
}