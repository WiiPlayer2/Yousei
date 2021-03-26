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

    internal class DumpAction : FlowAction<Unit>
    {
        private ILogger logger;

        public DumpAction(ILogger logger)
        {
            this.logger = logger;
        }

        protected override async Task Act(IFlowContext context, Unit arguments)
        {
            var contextObj = await context.AsObject();
            global::System.Diagnostics.Debug.WriteLine(contextObj);
            logger.LogDebug(contextObj.ToString());
        }
    }
}