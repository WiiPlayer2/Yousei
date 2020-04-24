using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Yousei.Modules
{
    public class MapModule : BaseOldModule
    {
        public string ID => "map";

        public override Task<IAsyncEnumerable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            return Task.FromResult(Map(arguments, data).YieldAsync());
        }

        private JToken Map(JToken value, JToken data) => value.Type switch
        {
            JTokenType.String => data.Get(value.Value<string>()),
            JTokenType.Object => GetObject(value as JObject, data),
            _ => value,
        };

        private JObject GetObject(JObject obj, JToken data)
        {
            var newObj = new JObject();
            obj.Properties().ForEach(prop => newObj.Add(prop.Name, Map(prop.Value, data)));
            return newObj;
        }
    }
}
