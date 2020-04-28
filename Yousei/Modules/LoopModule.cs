using LanguageExt;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    public class LoopModule : IModule
    {
        private class Arguments
        {
            public string Until { get; set; }

            public JobAction Action { get; set; }
        }

        private readonly ModuleRegistry moduleRegistry;

        public LoopModule(ModuleRegistry moduleRegistry)
        {
            this.moduleRegistry = moduleRegistry;
        }

        public async Task<IObservable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => { });
            cancellationToken.ThrowIfCancellationRequested();
            var args = arguments.ToObject<Arguments>();

            var moduleObservable = await moduleRegistry.RunAsync(args.Action.ModuleID, args.Action.Arguments, data, cancellationToken);
            return moduleObservable.SelectMany(async (JToken prevData, CancellationToken token) =>
            {
                token.ThrowIfCancellationRequested();
                if (prevData.Get(args.Until).ToObject<bool>())
                    return Observable.Return(prevData);
                return await ProcessAsync(arguments, data, token);
            }).SelectMany(o => o);
        }
    }
}
