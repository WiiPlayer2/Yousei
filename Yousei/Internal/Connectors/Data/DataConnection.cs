using Yousei.Core;

namespace Yousei.Internal.Connectors.Data
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