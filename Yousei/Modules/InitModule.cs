using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    public class InitModule : BaseOldModule
    {
        public string ID => "init";

        public override Task<IAsyncEnumerable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken) => Task.FromResult(arguments.YieldAsync());
    }
}
