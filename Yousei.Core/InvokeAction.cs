using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class InvokeAction : IFlowAction
    {
        private readonly Action action;

        public InvokeAction(Action action)
        {
            this.action = action;
        }

        public Type ArgumentsType { get; } = typeof(object);

        public Task Act(IFlowContext context, object? arguments)
        {
            action();
            return Task.CompletedTask;
        }
    }
}