using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal class IfAction : FlowAction<IfArguments>
    {
        protected override async Task Act(IFlowContext context, IfArguments arguments)
        {
            var condition = await arguments.If.Resolve<bool>(context);

            if (condition)
                using (context.ScopeStack("IF"))
                    await context.Actor.Act(arguments.Then, context);
            else
                using (context.ScopeStack("ELSE"))
                    await context.Actor.Act(arguments.Else, context);
        }
    }
}