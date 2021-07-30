using HotChocolate.Language;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Yousei.Api.Types
{
    internal class UnitType : ScalarType
    {
        public UnitType() : base("Unit")
        {
        }

        public override Type RuntimeType { get; } = typeof(Unit);

        public override bool IsInstanceOfType(IValueNode valueSyntax)
            => throw new NotImplementedException();

        public override object? ParseLiteral(IValueNode valueSyntax, bool withDefaults = true)
        {
            throw new NotImplementedException();
        }

        public override IValueNode ParseResult(object? resultValue)
        {
            throw new NotImplementedException();
        }

        public override IValueNode ParseValue(object? runtimeValue)
        {
            throw new NotImplementedException();
        }

        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            throw new NotImplementedException();
        }

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            resultValue = new Dictionary<string, object>();
            return true;
        }
    }
}