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
        public TriggerConnector() : base("trigger")
        {
        }

        protected override IConnection CreateConnection()
            => new TriggerConnection();
    }
}