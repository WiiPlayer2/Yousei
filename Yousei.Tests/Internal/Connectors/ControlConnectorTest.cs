using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using Yousei.Internal.Connectors.Control;
using System.Collections;

namespace Yousei.Test.Internal.Connectors
{
    [TestClass]
    public class ControlConnectorTest : ConnectorTest
    {
        [TestMethod]
        public async Task ForeachActionExecutesActionsForEveryItemInCollection()
        {
            // Arrange
            var actions = new List<BlockConfig>
            {
                new()
                {
                    Type = "testing.test",
                },
            };
            var collection = new object?[] { 1, 2, 3 }.ToList();
            var arguments = new ForEachArguments
            {
                Collection = collection,
                Actions = actions
            };

            // Act
            await Act("foreach", arguments);

            // Assert
            flowContextMock.Verify(o => o.SetData(It.IsIn<object?>(collection)), Times.Exactly(3));
            flowActorMock.Verify(o => o.Act(actions, flowContextMock.Object), Times.Exactly(3));
        }

        [TestMethod]
        public async Task IfActionExecutesElseIfConditionIsFalse()
        {
            // Arrange
            var @else = new List<BlockConfig>()
            {
                new()
                {
                    Type = "testing.test",
                }
            };
            var arguments = new IfArguments
            {
                If = false.ToConstantParameter(),
                Else = @else,
            };

            // Act
            await Act("if", arguments);

            // Assert
            flowActorMock.Verify(o => o.Act(@else, flowContextMock.Object));
        }

        [TestMethod]
        public async Task IfActionExecutesThenIfConditionIsTrue()
        {
            // Arrange
            var then = new List<BlockConfig>()
            {
                new()
                {
                    Type = "testing.test",
                }
            };
            var arguments = new IfArguments
            {
                If = true.ToConstantParameter(),
                Then = then,
            };

            // Act
            await Act("if", arguments);

            // Assert
            flowActorMock.Verify(o => o.Act(then, flowContextMock.Object));
        }

        [TestMethod]
        public async Task SwitchActionExecutesActionsBasedOnCase()
        {
            // Arrange
            var actions = new List<BlockConfig>
            {
                new()
                {
                    Type = "testing.test",
                },
            };
            var arguments = new SwitchArguments
            {
                Value = 1.ToConstantParameter(),
                Cases = new()
                {
                    (1, actions),
                }
            };

            // Act
            await Act("switch", arguments);

            // Assert
            flowActorMock.Verify(o => o.Act(actions, flowContextMock.Object));
        }

        [TestMethod]
        public async Task WhileActionExecutesActionsOnlyWhileConditionIsTrue()
        {
            // Arrange
            var conditionParameterMock = new Mock<IParameter<bool>>();
            conditionParameterMock.SetupSequence(o => o.Resolve(flowContextMock.Object))
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            var actions = new List<BlockConfig>
            {
                new()
                {
                    Type = "testing.test",
                },
            };
            var arguments = new WhileArguments
            {
                Condition = conditionParameterMock.Object,
                Actions = actions,
            };

            // Act
            await Act("while", arguments);

            // Assert
            conditionParameterMock.Verify(o => o.Resolve(flowContextMock.Object), Times.Exactly(3));
            flowActorMock.Verify(o => o.Act(actions, flowContextMock.Object), Times.Exactly(2));
        }

        #region Factory etc.

        protected override IConnector CreateConnector()
            => new ControlConnector();

        #endregion Factory etc.
    }
}