using System;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Control
{
    internal class WhileAction : FlowAction<UnitConnection, WhileArguments>
    {
        public override string Name { get; } = "while";

        protected override async Task Act(IFlowContext context, UnitConnection _, WhileArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            while (await arguments.Condition.Resolve<bool>(context))
            {
                await context.Actor.Act(arguments.Actions, context);
            }
        }
    }
}