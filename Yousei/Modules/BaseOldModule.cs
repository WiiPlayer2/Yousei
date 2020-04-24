using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    public abstract class BaseOldModule : IModule
    {
        public abstract Task<IAsyncEnumerable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken);

        async Task<IObservable<JToken>> IModule.ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
            => (await ProcessAsync(arguments, data, cancellationToken)).ToObservable();
    }
}
