using LanguageExt;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules.Templates
{
    public abstract class OptionTemplate : IModule
    {
        public abstract OptionAsync<JToken> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken);

        Task<IObservable<JToken>> IModule.ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
            => ProcessAsync(arguments, data, cancellationToken).Match(
                data => Observable.Return(data),
                () => Observable.Empty<JToken>());
    }
}
