using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Yousei.Modules
{
    class MapModule : IModule
    {
        public string ID => "map";

        public Task<IAsyncEnumerable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            return Task.FromResult(Map(arguments, data).YieldAsync());
        }

        private JToken Map(JToken value, JToken data) => value.Type switch
        {
            JTokenType.String => Get(data, value.Value<string>()),
            JTokenType.Object => GetObject(value as JObject, data),
            _ => value,
        };

        private JToken Get(JToken data, string path)
        {
            var parts = path.Split('.');
            return parts.Aggregate(data, (value, path) => value[path]);
        }

        private JObject GetObject(JObject obj, JToken data)
        {
            var newObj = new JObject();
            obj.Properties().ForEach(prop => newObj.Add(prop.Name, Map(prop.Value, data)));
            return newObj;
        }
    }
}
