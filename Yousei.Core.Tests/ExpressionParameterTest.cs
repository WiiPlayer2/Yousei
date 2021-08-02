using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core.Test
{
    [TestClass]
    public class ExpressionParameterTest
    {
        [TestMethod]
        public async Task ResolveWithExpressionReferencingContextReturnsValue()
        {
            // Arrange
            var flowContext = Mock.Of<IFlowContext>(o => o.AsObject().Result == JToken.FromObject(new { value = "henlo dere" })); // TODO: Fix for general objects
            var expression = "Context.value";
            var parameter = new ExpressionParameter<string>(expression);

            // Act
            var result = await parameter.Resolve(flowContext);

            // Assert
            result.Should().Be("henlo dere");
        }

        [TestMethod]
        public async Task ResolveWithValidExpressionReturnsValue()
        {
            // Arrange
            var expression = "return 1337;";
            var parameter = new ExpressionParameter<int>(expression);
            var flowContext = Mock.Of<IFlowContext>();

            // Act
            var result = await parameter.Resolve(flowContext);

            // Assert
            result.Should().Be(1337);
        }
    }
}