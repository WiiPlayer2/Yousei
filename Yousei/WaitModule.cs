using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Modules.Templates;

namespace Yousei
{
    public class WaitModule : SingleTemplate
    {
        public override async Task<JToken> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var intervalSeconds = arguments.ToObject<int>();
            await Task.Delay(intervalSeconds * 1000, cancellationToken);
            return data;
        }
    }
}
