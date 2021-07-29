using System;
using System.Collections;
using System.Linq;
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

            if (!arguments.Async)
            {
                foreach (var item in arguments.Collection)
                {
                    await context.SetData(item);
                    using (context.ScopeStack($"{{{item}}}"))
                        await context.Actor.Act(arguments.Actions, context);
                }
            }
            else
            {
                var tasks = arguments.Collection.Select(async item =>
                {
                    var subContext = context.Clone();
                    await subContext.SetData(item);
                    using (subContext.ScopeStack($"{{{item}}}"))
                        await subContext.Actor.Act(arguments.Actions, subContext);
                });
                await Task.WhenAll(tasks);
            }
        }
    }
}