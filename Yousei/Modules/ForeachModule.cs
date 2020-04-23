using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    class ForeachModule : BaseOldModule
    {
        public string ID => "foreach";

        public override Task<IAsyncEnumerable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var path = arguments.ToObject<string>();
            return Task.FromResult(data.Get(path).ToAsyncEnumerable());
        }
    }
}
