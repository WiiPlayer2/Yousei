using System;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Data
{
    internal class SetAction : FlowAction<UnitConnection, SetArguments>
    {
        public override string Name { get; } = "set";

        protected override async Task Act(IFlowContext context, UnitConnection _, SetArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var path = await arguments.Path.Resolve(context);
            var value = await arguments.Value.Resolve(context);

            if (path is null)
                throw new ArgumentNullException(nameof(arguments.Path));
            if (value is null)
                throw new ArgumentNullException(nameof(arguments.Value));

            await context.SetData(path, value);
        }
    }
}