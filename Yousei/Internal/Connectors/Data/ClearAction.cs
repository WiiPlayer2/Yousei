using System;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Internal;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Data
{
    internal class ClearAction : FlowAction<ClearArguments>
    {
        protected override async Task Act(IFlowContext context, ClearArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var path = await arguments.Path.Resolve<string>(context);

            if (path is null)
                throw new ArgumentNullException(nameof(arguments.Path));

            await context.ClearData(path);
        }
    }
}