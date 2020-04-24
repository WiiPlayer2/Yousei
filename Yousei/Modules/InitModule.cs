using LanguageExt;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Modules.Templates;

namespace Yousei.Modules
{
    public class InitModule : SingleTemplate
    {
        public override Task<JToken> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
            => arguments.AsTask();
    }
}
