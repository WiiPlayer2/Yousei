using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Modules.Templates;

namespace Yousei.Modules
{
    public class ForkModule : SingleTemplate
    {
        private class Arguments
        {
            public JToken Data { get; set; } = "$";

            public JobAction Action { get; set; }
        }

        private readonly ModuleRegistry moduleRegistry;

        public ForkModule(ModuleRegistry moduleRegistry)
        {
            this.moduleRegistry = moduleRegistry;
        }

        public override async Task<JToken> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var args = arguments.ToObject<Arguments>();
            var mappedData = args.Data.Map(data);
            var moduleObservable = await moduleRegistry.RunAsync(args.Action.ModuleID, args.Action.Arguments, mappedData, cancellationToken);
            moduleObservable.Wait();
            return data;
        }
    }
}
