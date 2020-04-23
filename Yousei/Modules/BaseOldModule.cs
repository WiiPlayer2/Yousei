using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    public abstract class BaseOldModule : IOldModule
    {
        public abstract Task<IAsyncEnumerable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken);

        async Task<IObservable<JToken>> IModule.Process(JToken arguments, JToken data, CancellationToken cancellationToken)
            => (await Process(arguments, data, cancellationToken)).ToObservable();
    }
}
