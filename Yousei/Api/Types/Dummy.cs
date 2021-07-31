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

        public static TValue? TryCast(object? obj)
        {
            if (obj is null)
                return default;

            var dummy = obj as Dummy<TValue, TExtra>;
            if (dummy is null)
                return default;

            return dummy.Value;
        }
    }
}