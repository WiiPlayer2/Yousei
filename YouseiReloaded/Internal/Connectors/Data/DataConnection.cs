using Yousei.Core;

namespace YouseiReloaded.Internal.Connectors.Data
{
    internal class DataConnection : SimpleConnection<DataConnection>
    {
        public DataConnection()
        {
            AddAction<SetAction>("set");
            AddAction<ClearAction>("clear");
        }
    }
}