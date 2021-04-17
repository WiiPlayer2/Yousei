using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using YouseiRelaoded.Internal.Connectors.Log;

namespace Yousei.Test.Internal.Connectors
{
    [TestClass]
    public class LogConnectorTest : ConnectorTest
    {
        private Mock<ILogger> loggerMock = new();

        [TestMethod]
        public async Task WriteActionWritesToLogger()
        {
            // Arrange
            var arguments = new WriteArguments
            {
                Level = LogLevel.Information.ToConstantParameter(),
                Tag = "test".ToConstantParameter(),
                Message = "idek".ToConstantParameter(),
            };

            // Act
            await Act("write", arguments);

            // Assert
            loggerMock.Verify(o => o.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));
        }

        protected override IConnection? CreateConnection(object? configuration = default)
            => CreateConnector().GetConnection(configuration);

        protected override IConnector CreateConnector()
            => new LogConnector(loggerMock.Object);
    }
}