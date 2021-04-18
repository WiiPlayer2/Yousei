using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using Yousei.Internal.Connectors.Data;

namespace Yousei.Test.Internal.Connectors
{
    [TestClass]
    public class DataConnectorTest : ConnectorTest
    {
        [TestMethod]
        public async Task ClearActionClearsDataInContext()
        {
            // Arrange
            var arguments = new ClearArguments
            {
                Path = "testing.tester.tested".ToConstantParameter(),
            };

            // Act
            await Act("clear", arguments);

            // Assert
            flowContextMock.Verify(o => o.ClearData("testing.tester.tested"));
        }

        [TestMethod]
        public async Task SetActionSetsDataInContext()
        {
            // Arrange
            var arguments = new SetArguments
            {
                Path = "testing.tester.tested".ToConstantParameter(),
                Value = 1337.ToConstantParameter(),
            };

            // Act
            await Act("set", arguments);

            // Assert
            flowContextMock.Verify(o => o.SetData("testing.tester.tested", 1337));
        }

        protected override IConnector CreateConnector()
            => new DataConnector();
    }
}