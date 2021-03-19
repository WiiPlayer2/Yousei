using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yousei.Contracts
{
    public interface IFlowActor
    {
        Task Act(IReadOnlyList<BlockConfig> actions, IFlowContext context);
    }
}