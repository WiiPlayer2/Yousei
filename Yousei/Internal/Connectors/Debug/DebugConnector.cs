using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Debug
{

    internal class DebugConnector : SingletonConnector
    {
        private readonly ILogger logger;

        public DebugConnector(ILogger logger) : base("debug")
        {
            this.logger = logger;
        }

        protected override IConnection CreateConnection()
            => new DebugConnection(logger);
    }
}