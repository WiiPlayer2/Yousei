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
    public class MappedParameterTest
    {
        [TestMethod]
        public async Task ResolveReturnsMappedValue()
        {
            // Arrange
            var flowContext = Mock.Of<IFlowContext>();
            var baseParameter = Mock.Of<IParameter>(o => o.Resolve(flowContext).Result == (object)"1337");
            var parameter = new MappedParameter<int>(baseParameter);

            // Act
            var result = await parameter.Resolve(flowContext);

            // Assert
            result.Should().Be(1337);
        }
    }
}