using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Yousei.Core.Serialization.Json;
using Yousei.Shared;

namespace Yousei.Core.Tests.Serialization.Json
{
    [TestClass]
    public class ParameterConverterTest
    {
        [TestMethod]
        public void ReadJsonShouldNotReturnParameterForNonParameterObject()
        {
            // Arrange
            var converter = new ParameterConverter();
            var type = typeof(IParameter);
            var json = @"{""asdf"":""1234""}";
            var expected = new ConstantParameter(JToken.FromObject(new { asdf = "1234" }));
            using var reader = new JsonTextReader(new StringReader(json));
            var serializer = Mock.Of<JsonSerializer>();

            // Act
            var result = converter.ReadJson(reader, type, default, serializer);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}