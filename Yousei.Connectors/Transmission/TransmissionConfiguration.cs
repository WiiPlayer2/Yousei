using System;

namespace Yousei.Connectors.Transmission
{
    public record TransmissionConfiguration
    {
        public Uri Endpoint { get; init; }
        public string Login { get; init; }
        public string Password { get; init; }
    }
}