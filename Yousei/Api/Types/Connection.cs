using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yousei.Api.Types
{
    public record Connection(string Id, IReadOnlyList<Configuration> Configurations);
}