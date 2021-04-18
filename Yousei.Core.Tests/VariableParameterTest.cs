using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core.Test
{
    [TestClass]
    public class VariableParameterTest
    {
        [TestMethod]
        public async Task ResolveReturnsValueFromContextAtPath()
        {
            // Arrange
            var path = "testing.test";
            var parameter = new VariableParameter(path);
            var flowContext = Mock.Of<IFlowContext>(o => o.GetData(path).Result == (object)69420);

            // Act
            var result = await parameter.Resolve<int>(flowContext);

            // Assert
            result.Should().Be(69420);
        }
    }
}