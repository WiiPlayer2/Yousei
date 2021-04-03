using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace YouseiReloaded.Internal.Connectors.Internal
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