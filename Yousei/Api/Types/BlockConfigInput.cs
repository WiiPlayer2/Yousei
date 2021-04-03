using Newtonsoft.Json.Linq;
using Yousei.Shared;

namespace Yousei.Api.Types
{
    public record BlockConfigInput(string Type, string Configuration = "default", JToken arguments = default)
    {
        public BlockConfig Map()
            => new()
            {
                Type = Type,
                Configuration = Configuration,
                Arguments = arguments,
            };
    }
}