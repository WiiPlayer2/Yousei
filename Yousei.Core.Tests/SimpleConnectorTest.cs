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

namespace Yousei.Core.Test
{
    [TestClass]
    public class SimpleConnectorTest
    {
        [TestMethod]
        public void CreateConnectionReceivesNonNullConfiguration()
        {
            // Arrange
            var connector = new TestConnector();
            var config = new object();

            // Act
            connector.GetConnection(config);

            // Assert
            connector.LastReceived.Should().Be((1, config));
        }

        [TestMethod]
        public void GetConnectionReturnsDefaultConnectionForNullConfiguration()
        {
            // Arrange
            var connector = new TestConnector();
            var config = default(object);
            var defaultConnection = Mock.Of<IConnection>();
            connector.DefaultConnection = defaultConnection;

            // Act
            var result = connector.GetConnection(config);

            // Assert
            using (new AssertionScope())
            {
                result.Should().Be(defaultConnection);
                connector.LastReceived.Received.Should().Be(0);
            }
        }

        [TestMethod]
        public void GetConnectionReturnsSameConnectionForSameConfiguration()
        {
            // Arrange
            var connector = new TestConnector();
            var config = new object();

            // Act
            var result1 = connector.GetConnection(config);
            var result2 = connector.GetConnection(config);

            // Assert
            using (new AssertionScope())
            {
                result1.Should().BeSameAs(result2);
                connector.LastReceived.Received.Should().Be(1);
            }
        }

        private class TestConnector : SimpleConnector<object>
        {
            public TestConnector() : base("test")
            {
            }

            public new IConnection? DefaultConnection
            {
                get => base.DefaultConnection;
                set => base.DefaultConnection = value;
            }

            public (int Received, object? Configuration) LastReceived { get; private set; }

            protected override IConnection? CreateConnection(object configuration)
            {
                LastReceived = (LastReceived.Received + 1, configuration);
                return Mock.Of<IConnection>();
            }
        }
    }
}