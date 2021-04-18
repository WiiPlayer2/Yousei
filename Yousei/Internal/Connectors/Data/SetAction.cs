using System;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Data
{
    internal class SetAction : FlowAction<SetArguments>
    {
        protected override async Task Act(IFlowContext context, SetArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var path = await arguments.Path.Resolve<string>(context);
            var value = await arguments.Value.Resolve<object>(context);

            if (path is null)
                throw new ArgumentNullException(nameof(arguments.Path));
            if (value is null)
                throw new ArgumentNullException(nameof(arguments.Value));

            await context.SetData(path, value);
        }
    }
}