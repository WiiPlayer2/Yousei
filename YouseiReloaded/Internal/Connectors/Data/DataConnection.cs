using Yousei.Core;

namespace YouseiReloaded.Internal.Connectors.Data
{
    internal class DataConnection : SimpleConnection
    {
        public DataConnection()
        {
            AddAction<SetAction>("set");
            AddAction<ClearAction>("clear");
        }
    }
}