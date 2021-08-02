using Newtonsoft.Json.Linq;
using StrawberryShake.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yousei.Web.Api.Serialization
{
    internal class JsonStringSerializer : ScalarSerializer<string, JToken>
    {
        public JsonStringSerializer() : base("JsonString")
        {
        }

        public override JToken Parse(string serializedValue)
            => JToken.Parse(serializedValue);

        protected override string Format(JToken runtimeValue)
            => runtimeValue.ToString(Newtonsoft.Json.Formatting.None);
    }
}