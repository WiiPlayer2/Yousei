using Yousei.Core;

namespace YouseiReloaded.Internal.Connectors.Trigger
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