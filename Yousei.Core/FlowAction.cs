using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class FlowAction<TArguments> : IFlowAction
    {
        public Type ArgumentsType { get; } = typeof(TArguments);

        public Task Act(IFlowContext context, object arguments)
            => Act(context, arguments.SafeCast<TArguments>());

        protected abstract Task Act(IFlowContext context, TArguments arguments);
    }
}