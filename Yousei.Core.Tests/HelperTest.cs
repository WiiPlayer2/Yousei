using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core.Tests
{
    [TestClass]
    public class HelperTest
    {
        [TestMethod]
        public async Task IgnoreCancellation_DoesNotThrowWhenCancelled()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            var task = Task.Delay(10000, cts.Token);
            cts.Cancel();

            // Act
            var act = new Func<Task>(() => task.IgnoreCancellation());

            // Assert
            await act.Should().NotThrowAsync<OperationCanceledException>();
        }

        [TestMethod]
        public void Map_ReturnsMappedParameterWithCorrectType()
        {
            // Arrange
            var baseParameter = Mock.Of<IParameter>();

            // Act
            var result = baseParameter.Map<int>();

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<MappedParameter<int>>();
                ((MappedParameter<int>)result).Base.Should().BeSameAs(baseParameter);
            }
        }

        [TestMethod]
        public void Map_ReturnsParameterForObjectType()
        {
            // Arrange
            var baseParameter = Mock.Of<IParameter>();

            // Act
            var result = baseParameter.Map(typeof(object));

            // Assert
            result.Should().BeSameAs(baseParameter);
        }

        [DataRow("asdf.qwertz", "asdf", "qwertz")]
        [DataTestMethod]
        public void SplitType_ReturnsSplitTypeIfValid(string type, string expectedConnectorName, string expectedName)
        {
            // Act
            var result = type.SplitType();

            // Assert
            result.Should().BeEquivalentTo((expectedConnectorName, expectedName));
        }

        [DataRow("")]
        [DataRow("asdf")]
        [DataRow("asdf.qwertz.jkl")]
        [DataTestMethod]
        public void SplitType_ThrowsExceptionIfInvalid(string type)
        {
            // Act
            var act = new Action(() => type.SplitType());

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void ThrowIfNull_DoesNotThrowIfValueIsNotNull()
        {
            // Arrange
            int? value = 1234;

            // Act
            Action act = () => value.ThrowIfNull();

            // Assert
            act.Should().NotThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void ThrowIfNull_ThrowsIfValueIsNull()
        {
            // Arrange
            int? value = null;

            // Act
            Action act = () => value.ThrowIfNull();

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void TryGetValue_ReturnsFalseIfNotFound()
        {
            // Arrange
            var dict = Mock.Of<DictionaryBase>();
            var key = "testKey";

            // Act
            var result = dict.TryGetValue(key, out var resultValue);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void TryGetValue_ReturnsTrueAndValueIfFound()
        {
            // Arrange
            var dict = Mock.Of<DictionaryBase>();
            var key = "testKey";
            var value = "testValue";
            ((IDictionary)dict)[key] = value;

            // Act
            var result = dict.TryGetValue(key, out var resultValue);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                resultValue.Should().BeEquivalentTo(value);
            }
        }
    }
}