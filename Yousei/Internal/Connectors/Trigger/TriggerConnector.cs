using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Trigger
{
    internal class TriggerConnector : SingletonConnector
    {
        public TriggerConnector()
        {
            AddTrigger<DistinctTrigger>();
            AddTrigger<PeriodicTrigger>();
            AddTrigger<WhenAnyTrigger>();
        }

        public override string Name { get; } = "trigger";
    }
}