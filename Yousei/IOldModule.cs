using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei
{
    interface IModule
    {
        Task<IObservable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken);
    }

    interface IOldModule : IModule
    {
        new Task<IAsyncEnumerable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken);
    }
}
