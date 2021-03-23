using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Data
{
    internal class SetAction : FlowAction<SetArguments>
    {
        protected override async Task Act(IFlowContext context, SetArguments arguments)
        {
            var path = await arguments.Path.Resolve<string>(context);
            var value = await arguments.Value.Resolve<object>(context);
            await context.SetData(path, value);
        }
    }
}