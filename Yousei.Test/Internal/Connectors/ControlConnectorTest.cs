using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using YouseiReloaded.Internal.Connectors.Control;

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
            var collection = new[] { 1, 2, 3 };
            var arguments = new ForEachArguments
            {
                Collection = collection.ToConstantParameter(),
                Actions = actions
            };

            // Act
            await Act("foreach", arguments);

            // Assert
            flowContextMock.Verify(o => o.SetData(It.IsIn(collection)), Times.Exactly(3));
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
            var conditionParameterMock = new Mock<IParameter>();
            conditionParameterMock.SetupSequence(o => o.Resolve<bool>(flowContextMock.Object))
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
            conditionParameterMock.Verify(o => o.Resolve<bool>(flowContextMock.Object), Times.Exactly(3));
            flowActorMock.Verify(o => o.Act(actions, flowContextMock.Object), Times.Exactly(2));
        }

        #region Factory etc.

        protected override IConnector CreateConnector()
            => new ControlConnector();

        #endregion Factory etc.
    }
}