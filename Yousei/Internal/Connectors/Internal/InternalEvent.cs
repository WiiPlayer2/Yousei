using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Yousei.Internal.Connectors.Internal
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum InternalEvent
    {
        Start,

        Stop,

        Exception,

        Reloading,

        Reloaded,

        FlowAdded,

        FlowUpdated,

        FlowRemoved,
    }
}