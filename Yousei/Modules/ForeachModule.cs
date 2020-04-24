using LanguageExt;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Modules.Templates;

namespace Yousei.Modules
{
    public class ForeachModule : EnumerableTemplate
    {
        public override Task<IEnumerable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var path = arguments.ToObject<string>();
            return data.Get(path).AsTask<IEnumerable<JToken>>();
        }
    }
}
