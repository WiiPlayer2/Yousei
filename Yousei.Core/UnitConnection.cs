using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public struct UnitConnection : IConnection
    {
        public static UnitConnection Default { get; } = default;

        public void Dispose()
        {
        }
    }
}