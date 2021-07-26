using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class InvokeAction : IFlowAction
    {
        private readonly Action action;

        public InvokeAction(string name, Action action)
        {
            Name = name;
            this.action = action;
        }

        public Type ArgumentsType { get; } = typeof(Unit);

        public string Name { get; }

        public Task Act(IFlowContext context, IConnection connection, object? arguments)
        {
            action();
            return Task.CompletedTask;
        }
    }
}