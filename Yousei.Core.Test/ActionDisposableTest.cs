using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Yousei.Core.Test
{
    [TestClass]
    public class ActionDisposableTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var flag = false;
            var action = new Action(() => flag = true);
            var actionDisposable = new ActionDisposable(action);

            // Act
            actionDisposable.Dispose();

            // Assert
            flag.Should().BeTrue();
        }
    }
}