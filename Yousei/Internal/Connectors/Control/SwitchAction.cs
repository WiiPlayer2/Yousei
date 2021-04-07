using System;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal class SwitchAction : FlowAction<SwitchArguments>
    {
        protected override async Task Act(IFlowContext context, SwitchArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var value = await arguments.Value.Resolve<object>(context);

            foreach (var (@case, actions) in arguments.Cases)
            {
                if (Equals(value, @case))
                {
                    using (context.ScopeStack($"CASE {{{@case}}}"))
                        await context.Actor.Act(actions, context);
                    return;
                }
            }

            using (context.ScopeStack("DEFAULT"))
                await context.Actor.Act(arguments.Default, context);
        }
    }
}