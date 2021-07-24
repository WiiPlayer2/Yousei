using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IFlowContextFactory
    {
        IFlowContext Create(IFlowActor actor, string flowName);
    }
}