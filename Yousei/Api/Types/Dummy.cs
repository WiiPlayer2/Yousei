using HotChocolate.Language;
using HotChocolate.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yousei.Api.Types
{
    internal record Dummy<TValue, TExtra>(TValue Value)
    {
        public static implicit operator Dummy<TValue, TExtra>(TValue value)
            => new(value);

        public static implicit operator TValue(Dummy<TValue, TExtra> dummy)
            => dummy.Value;
    }
}