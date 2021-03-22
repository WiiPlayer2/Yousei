using System.Collections.Generic;

namespace Yousei.Connectors.Http
{
    public record Config
    {
        public List<string> Prefixes { get; init; } = new();
    }
}