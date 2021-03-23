using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Data
{
    internal class ClearAction : FlowAction<ClearArguments>
    {
        protected override async Task Act(IFlowContext context, ClearArguments arguments)
        {
            var path = await arguments.Path.Resolve<string>(context);
            await context.ClearData(path);
        }
    }
}