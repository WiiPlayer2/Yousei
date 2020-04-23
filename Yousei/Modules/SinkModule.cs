using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    public class SinkModule : IModule
    {
        public Task<IObservable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken)
            => Task.FromResult(Observable.Empty<JToken>());
    }
}
