using NuxeoClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Nuxeo
{
    internal abstract class NuxeoAction<T> : FlowAction<ObjectConnection<NuxeoConfig>, T>
    {
        protected sealed override async Task Act(IFlowContext context, ObjectConnection<NuxeoConfig> connection, T? arguments)
        {
            var config = connection.Object;
            using var client = new Client(config.Url, new(config.Username, config.Password));
            await Act(context, arguments, client);
        }

        protected abstract Task Act(IFlowContext context, T? arguments, Client client);
    }
}