using Newtonsoft.Json.Linq;
using StrawberryShake.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Yousei.Web.Api.Serialization
{
    internal class UnitSerializer : ScalarSerializer<string, Unit>
    {
        public UnitSerializer() : base("Unit")
        {
        }

        public override Unit Parse(string serializedValue)
        {
            throw new NotImplementedException();
        }

        protected override string Format(Unit runtimeValue)
        {
            throw new NotImplementedException();
        }
    }
}