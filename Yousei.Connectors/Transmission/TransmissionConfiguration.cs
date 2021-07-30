using System;

namespace Yousei.Connectors.Transmission
{
    public record TransmissionConfiguration
    {
        public Uri? Endpoint { get; init; }

        public string Login { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }
}