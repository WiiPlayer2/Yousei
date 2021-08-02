using System.Reactive;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Control
{
    internal class ControlConnector : SingletonConnector
    {
        public ControlConnector()
        {
            AddAction<IfAction>();
            AddAction<ForEachAction>();
            AddAction<WhileAction>();
            AddAction<SwitchAction>();
            AddAction<TryAction>();
        }

        public override string Name { get; } = "control";
    }
}