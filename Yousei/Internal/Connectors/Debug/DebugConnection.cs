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
    internal class DebugConnection : SimpleConnection
    {
        public DebugConnection(ILogger logger)
        {
            AddAction("dump", new DumpAction(logger));
            AddAction("break", new InvokeAction(Debugger.Break));
        }
    }
}