using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yousei.Web.Api
{
    public record ApiOptions
    {
        public Uri Url { get; init; }
    }
}