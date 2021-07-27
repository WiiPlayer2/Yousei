using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Data
{
    internal class DataConnector : SingletonConnector
    {
        public DataConnector()
        {
            AddAction<SetAction>();
            AddAction<ClearAction>();
        }

        public override string Name { get; } = "data";
    }
}