using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yousei.Core.Tests
{
    [TestClass]
    public class HelperTest
    {
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