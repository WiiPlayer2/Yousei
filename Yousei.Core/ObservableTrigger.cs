using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class ObservableTrigger : ObservableTrigger<IConnection>
    {
        public ObservableTrigger(string name, Func<IConnection, IObservable<object>> observableFunc) : base(name, observableFunc)
        {
        }

        public ObservableTrigger(string name, IObservable<object> observable) : base(name, observable)
        {
        }
    }

    public class ObservableTrigger<TConnection> : IFlowTrigger
        where TConnection : IConnection
    {
        private readonly Func<TConnection, IObservable<object>> observableFunc;

        public ObservableTrigger(string name, Func<TConnection, IObservable<object>> observableFunc)
        {
            Name = name;
            this.observableFunc = observableFunc;
        }

        public ObservableTrigger(string name, IObservable<object> observable)
            : this(name, _ => observable)
        {
        }

        public Type ArgumentsType { get; } = typeof(object);

        public string Name { get; }

        public IObservable<object> GetEvents(IFlowContext context, IConnection connection, object? arguments)
            => observableFunc((TConnection)connection);
    }
}