using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IFlowActor
    {
        Task Act(IReadOnlyList<BlockConfig> actions, IFlowContext context);

        IObservable<object> GetTrigger(BlockConfig trigger);
    }
}