﻿using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Control
{
    internal class SwitchAction : FlowAction<SwitchArguments>
    {
        protected override async Task Act(IFlowContext context, SwitchArguments arguments)
        {
            var value = await arguments.Value.Resolve<object>(context);

            foreach (var (@case, actions) in arguments.Cases)
            {
                if (Equals(value, @case))
                {
                    await context.Actor.Act(actions, context);
                    return;
                }
            }

            await context.Actor.Act(arguments.Default, context);
        }
    }
}