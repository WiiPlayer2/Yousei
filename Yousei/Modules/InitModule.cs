using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    class InitModule : BaseOldModule
    {
        public string ID => "init";

        public override Task<IAsyncEnumerable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken) => Task.FromResult(arguments.YieldAsync());
    }
}
