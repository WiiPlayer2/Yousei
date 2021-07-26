using Newtonsoft.Json.Linq;
using NuxeoClient;
using NuxeoClient.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Nuxeo
{

    internal class NuxeoConnection : SimpleConnection
    {
        public NuxeoConnection(NuxeoConfig config)
        {
            AddAction("get_document", new DocumentGetAction(config));
            AddAction("rest", new RestAction(config));
        }
    }
}