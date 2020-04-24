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
    public class SinkModule : VoidTemplate
    {
        public override Task ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
