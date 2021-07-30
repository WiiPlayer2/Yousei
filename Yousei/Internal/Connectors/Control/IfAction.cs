using System;
using System.Reactive;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Control
{
    internal class IfAction : FlowAction<UnitConnection, IfArguments>
    {
        public override string Name { get; } = "if";

        protected override async Task Act(IFlowContext context, UnitConnection _, IfArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var condition = await arguments.If.Resolve(context);

            if (condition)
                using (context.ScopeStack("IF"))
                    await context.Actor.Act(arguments.Then, context);
            else
                using (context.ScopeStack("ELSE"))
                    await context.Actor.Act(arguments.Else, context);
        }
    }
}