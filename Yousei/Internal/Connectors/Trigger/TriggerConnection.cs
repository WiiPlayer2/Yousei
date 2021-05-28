using Yousei.Core;

namespace Yousei.Internal.Connectors.Trigger
{
    internal class TriggerConnection : SimpleConnection
    {
        public TriggerConnection()
        {
            AddTrigger<DistinctTrigger>("distinct");
            AddTrigger<PeriodicTrigger>("periodic");
            AddTrigger<WhenAnyTrigger>("whenany");
        }
    }
}