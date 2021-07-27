using System;
using System.Collections;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Control
{
    internal class ForEachAction : FlowAction<UnitConnection, ForEachArguments>
    {
        public override string Name { get; } = "foreach";

        protected override async Task Act(IFlowContext context, UnitConnection _, ForEachArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var collection = await arguments.Collection.Resolve<ArrayList>(context);
            if (collection is null)
                throw new ArgumentNullException(nameof(arguments.Collection));

            foreach (var item in collection)
            {
                await context.SetData(item);
                using (context.ScopeStack($"{{{item}}}"))
                    await context.Actor.Act(arguments.Actions, context);
            }
        }
    }
}