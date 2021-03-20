using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class SimpleConnection<TConnection> : IConnection
        where TConnection : SimpleConnection<TConnection>
    {
        private readonly Dictionary<string, Func<TConnection, IFlowAction>> actionTypes = new();

        private readonly Dictionary<string, Func<TConnection, IFlowTrigger>> triggerTypes = new();

        public IFlowAction CreateAction(string name)
        {
            if (actionTypes.TryGetValue(name, out var creator))
                return creator(this as TConnection);
            return default;
        }

        public IFlowTrigger CreateTrigger(string name)
        {
            if (triggerTypes.TryGetValue(name, out var creator))
                return creator(this as TConnection);
            return default;
        }

        protected void AddAction<T>(string name)
            where T : IFlowAction
            => AddAction(name, GetInstanceCreator<T, IFlowAction>());

        protected void AddAction(string name, Func<TConnection, IFlowAction> creator)
            => actionTypes.Add(name, creator);

        protected void AddTrigger<T>(string name)
            where T : IFlowTrigger
            => AddTrigger(name, GetInstanceCreator<T, IFlowTrigger>());

        protected void AddTrigger(string name, Func<TConnection, IFlowTrigger> creator)
            => triggerTypes.Add(name, creator);

        private static Func<TConnection, TReturn> GetInstanceCreator<T, TReturn>()
            where T : TReturn
            => _ => Activator.CreateInstance<T>();
    }
}