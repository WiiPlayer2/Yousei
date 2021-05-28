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
    public class SimpleConnectionTest
    {
        [TestMethod]
        public void ReturnsActionForNameWhenAvailable()
        {
            // Arrange
            var action = Mock.Of<IFlowAction>();
            var connection = new TestConnection();

            // Act
            connection.AddAction("test", action);
            var result = connection.CreateAction("test");

            // Assert
            result.Should().BeSameAs(action);
        }

        [TestMethod]
        public void ReturnsNullAsActionForNameWhenUnavailable()
        {
            // Arrange
            var action = Mock.Of<IFlowAction>();
            var connection = new TestConnection();

            // Act
            var result = connection.CreateAction("test");

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void ReturnsNullAsTriggerForNameWhenUnavailable()
        {
            // Arrange
            var trigger = Mock.Of<IFlowTrigger>();
            var connection = new TestConnection();

            // Act
            var result = connection.CreateTrigger("test");

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void ReturnsTriggerForNameWhenAvailable()
        {
            // Arrange
            var trigger = Mock.Of<IFlowTrigger>();
            var connection = new TestConnection();

            // Act
            connection.AddTrigger("test", trigger);
            var result = connection.CreateTrigger("test");

            // Assert
            result.Should().BeSameAs(trigger);
        }

        private class TestConnection : SimpleConnection
        {
            public new void AddAction(string name, IFlowAction instance)
                => base.AddAction(name, instance);

            public new void AddTrigger(string name, IFlowTrigger instance)
                => base.AddTrigger(name, instance);
        }
    }
}