using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei
{
    public interface IModule
    {
        Task<IObservable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken);
    }

    public interface IOldModule : IModule
    {
        new Task<IAsyncEnumerable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken);
    }
}
