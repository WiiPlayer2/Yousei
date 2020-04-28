using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Yousei.Modules.Templates;
using LanguageExt;

namespace Yousei.Modules
{
    public class MapModule : SingleTemplate
    {
        public override Task<JToken> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
            => arguments.Map(data).AsTask();
    }
}
