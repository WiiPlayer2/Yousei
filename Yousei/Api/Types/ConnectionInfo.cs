using HotChocolate.Configuration;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;

namespace Yousei.Api.Types
{

    public class ConnectionInfo : Wrapper<IConnection>
    {
        public ConnectionInfo(IConnection connection) : base(connection)
        { }

        public static ConnectionInfo? From(IConnection? connection)
            => connection is not null
                ? new ConnectionInfo(connection)
                : null;
    }
}