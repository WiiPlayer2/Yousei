using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal class WhileAction : FlowAction<WhileArguments>
    {
        protected override async Task Act(IFlowContext context, WhileArguments arguments)
        {
            while (await arguments.Condition.Resolve<bool>(context))
            {
                await context.Actor.Act(arguments.Actions, context);
            }
        }
    }
}