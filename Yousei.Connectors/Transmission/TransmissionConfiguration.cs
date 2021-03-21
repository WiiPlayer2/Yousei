using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yousei.Connectors.Transmission
{
    public record TransmissionConfiguration
    {
        public string Endpoint { get; init; }
        public string Login { get; init; }
        public string Password { get; init; }
    }
}
