using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;
using Yousei.Internal;

namespace Yousei.Test.Internal
{
    [TestClass]
    public class FlowActorTest
    {
        [TestMethod]
        public async Task ExecutesConnectionActionForGivenBlockConfig()
        {
            // Arrange
            var actor = CreateActor();
            var actions = new BlockConfig[]
            {
                new()
                {
                    Type = "testing.test",
                },
            };
            var testConnectorMock = new Mock<IConnector>();
            var testConnectionMock = new Mock<IConnection>();
            var testActionMock = new Mock<IFlowAction>();
            connectorRegistryMock.Setup(o => o.Get("testing")).Returns(testConnectorMock.Object);
            testConnectorMock.Setup(o => o.GetConnection(It.IsAny<object?>())).Returns(testConnectionMock.Object);
            testConnectionMock.Setup(o => o.CreateAction("test")).Returns(testActionMock.Object);

            // Act
            await actor.Act(actions, flowContextMock.Object);

            // Assert
            testActionMock.Verify(o => o.Act(flowContextMock.Object, It.IsAny<object?>()));
        }

        [TestMethod]
        public void GetsConnectionTriggerForGivenBlockConfig()
        {
            // Arrange
            var actor = CreateActor();
            var trigger = new BlockConfig
            {
                Type = "testing.test",
            };
            var testConnectorMock = new Mock<IConnector>();
            var testConnectionMock = new Mock<IConnection>();
            var testTriggerMock = new Mock<IFlowTrigger>();
            var testObservable = Observable.Empty<object>();
            connectorRegistryMock.Setup(o => o.Get("testing")).Returns(testConnectorMock.Object);
            testConnectorMock.Setup(o => o.GetConnection(It.IsAny<object?>())).Returns(testConnectionMock.Object);
            testConnectionMock.Setup(o => o.CreateTrigger("test")).Returns(testTriggerMock.Object);
            testTriggerMock.Setup(o => o.GetEvents(flowContextMock.Object, It.IsAny<object?>())).Returns(testObservable);

            // Act
            var result = actor.GetTrigger(trigger, flowContextMock.Object);

            // Assert
            testTriggerMock.Verify(o => o.GetEvents(flowContextMock.Object, It.IsAny<object?>()));
            result.Should().BeSameAs(testObservable);
        }

        #region Factory etc.

        private readonly Mock<IConfigurationProvider> configurationProviderMock = new();

        private readonly Mock<IConnectorRegistry> connectorRegistryMock = new();

        private readonly Mock<IFlowContext> flowContextMock = new();

        [TestInitialize]
        public void Initialize()
        {
            flowContextMock.Setup(o => o.ExecutionStack).Returns(new Stack<string>());
        }

        private FlowActor CreateActor()
            => new FlowActor(configurationProviderMock.Object, connectorRegistryMock.Object);

        #endregion Factory etc.
    }
}