using HotChocolate.Configuration;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Api.Types
{

    public class Wrapper<T>
    {
        public Wrapper(T wrapped)
        {
            Wrapped = wrapped;
        }

        public T Wrapped { get; }
    }
}