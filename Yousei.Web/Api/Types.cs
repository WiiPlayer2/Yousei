using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Web.Api
{
    internal record Database(bool? IsReadOnly, Flow? Flow, Configuration? Configuration, Connection[]? Connections, Flow[]? Flows);

    internal record Flow(string? Name, SourceConfig? Config);

    internal record Configuration(string? Name, SourceConfig? Config);

    internal record Connection(string? Id, Configuration[]? Configurations);

    internal record Query(Database? Database);
}