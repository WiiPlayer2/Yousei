using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class SimpleConnection : IConnection
    {
        private readonly Dictionary<string, IFlowAction> actions = new();

        private readonly Dictionary<string, IFlowTrigger> triggers = new();

        public IFlowAction CreateAction(string name)
        {
            if (actions.TryGetValue(name, out var action))
                return action;
            return default;
        }

        public IFlowTrigger CreateTrigger(string name)
        {
            if (triggers.TryGetValue(name, out var trigger))
                return trigger;
            return default;
        }

        protected void AddAction<T>(string name)
            where T : IFlowAction
            => AddAction(name, Activator.CreateInstance<T>());

        protected void AddAction(string name, IFlowAction instance)
            => actions.Add(name, instance);

        protected void AddTrigger<T>(string name)
            where T : IFlowTrigger
            => AddTrigger(name, Activator.CreateInstance<T>());

        protected void AddTrigger(string name, IFlowTrigger instance)
            => triggers.Add(name, instance);
    }
}