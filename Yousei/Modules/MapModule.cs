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
            => Map(arguments, data).AsTask();

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
