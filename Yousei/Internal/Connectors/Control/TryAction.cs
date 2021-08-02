using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Control
{
    internal record TryArguments
    {
        public List<BlockConfig> Try { get; init; } = new();

        public List<BlockConfig> Catch { get; init; } = new();

        public List<BlockConfig> Finally { get; init; } = new();
    }

    internal class TryAction : FlowAction<TryArguments>
    {
        public override string Name { get; } = "try";

        protected override async Task Act(IFlowContext context, TryArguments? arguments)
        {
            arguments.ThrowIfNull();

            try
            {
                using (context.ScopeStack("TRY"))
                    await context.Actor.Act(arguments.Try, context);
            }
            catch (Exception e)
            {
                await context.SetData(e);
                using (context.ScopeStack("CATCH"))
                    await context.Actor.Act(arguments.Catch, context);
            }
            finally
            {
                using (context.ScopeStack("FINALLY"))
                    await context.Actor.Act(arguments.Finally, context);
            }
        }
    }
}