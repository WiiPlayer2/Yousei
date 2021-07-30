using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Test.Internal
{
    [TestClass]
    public abstract class FlowContextTest
    {
        [TestMethod]
        public async Task CloneReturnsEquivalentContext()
        {
            // Arrange
            var context = CreateContext();
            await context.SetData("testing", "test");
            context.CurrentType = "testing.type";
            var sourceObject = await context.AsObject();

            // Act
            var result = context.Clone();
            var resultObject = await result.AsObject();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeSameAs(context);
                result.Should().BeEquivalentTo(context);
                resultObject.Should().NotBeSameAs(sourceObject);
                resultObject.Should().BeEquivalentTo(sourceObject);
            }
        }

        [TestMethod]
        public async Task OverwritingDataActuallyOverwritesData()
        {
            // Arrange
            var context = CreateContext();
            await context.SetData("testing.test", 123);

            // Act
            await context.SetData("testing.test", 456);

            // Assert
            dynamic data = await context.AsObject();
            int value = data.testing.test;
            value.Should().Be(456);
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

        protected readonly Mock<IFlowActor> flowActorMock = new();

        protected abstract IFlowContext CreateContext();

        #endregion Factory etc.
    }
}