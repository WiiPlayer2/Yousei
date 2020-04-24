using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules.Templates
{
    public abstract class SingleTemplate : IModule
    {
        public abstract Task<JToken> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken);

        async Task<IObservable<JToken>> IModule.ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
            => Observable.Return(await ProcessAsync(arguments, data, cancellationToken).ConfigureAwait(false));
    }
}
