using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yousei.Shared;
using YouseiReloaded.Dummy;
using YouseiReloaded.Internal;
using YouseiReloaded.Serialization.Json;

namespace YouseiReloaded
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new[]
                {
                    new ParameterConverter(),
                }
            };

            var blockConfigs = new BlockConfig[]
            {
                new()
                {
                    Type = "console.out",
                    Arguments = new Dictionary<string, object>
                    {
                        { "text", new ExpressionParameter(@"$""asdf: {Context.http.webhook}""") },
                    },
                }
            };

            var actor = new FlowActor(new DummyConfigurationProvider(), new DummyConnectorRegistry());
            var context = new FlowContext(actor);
            await context.AddData("http.webhook", JToken.FromObject("this is definitely a http body"));
            await actor.Act(blockConfigs, context);
        }
    }
}