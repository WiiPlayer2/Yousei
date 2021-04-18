using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;
using YouseiReloaded.Internal;

namespace Yousei.Test.Internal
{
    [TestClass]
    public class FlowContextTest
    {
        [TestMethod]
        public async Task CloneReturnsEquivalentContext()
        {
            // Arrange
            var context = CreateContext();
            await context.SetData("testing", "test");

            // Act
            var result = context.Clone();

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(context);
                (await result.AsObject()).Should().BeEquivalentTo(await context.AsObject());
            }
        }

        [TestMethod]
        public async Task SetDataSetsDataAtSpecifiedPath()
        {
            // Arrange
            var context = CreateContext();

            // Act
            await context.SetData("testing.test", 123);

            // Assert
            dynamic data = await context.AsObject();
            int value = data.testing.test;
            value.Should().Be(123);
        }

        #region Factory etc.

        private readonly Mock<IFlowActor> flowActorMock = new();

        private FlowContext CreateContext()
            => new FlowContext(flowActorMock.Object, "testing");

        #endregion Factory etc.
    }
}