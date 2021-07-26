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
        public DebugConnector(ILogger logger)
        {
            AddAction(new DumpAction(logger));
            AddAction(new InvokeAction("break", Debugger.Break));
        }

        public override string Name { get; } = "debug";
    }
}