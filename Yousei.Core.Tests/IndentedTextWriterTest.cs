using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Yousei.Core.Tests
{
    [TestClass]
    public class IndentedTextWriterTest
    {
        [TestMethod]
        public void IndentIndentsMultipleLines()
        {
            // Arrange
            var baseWriter = new StringWriter();
            var writer = new IndentedTextWriter(baseWriter, 4);

            // Act
            using (writer.Indent())
            {
                writer.Write("asdf\njklo");
            }

            // Assert
            baseWriter.ToString().Should().Be("    asdf\n    jklo");
        }

        [TestMethod]
        public void IndentIndentsText()
        {
            // Arrange
            var baseWriter = new StringWriter();
            var writer = new IndentedTextWriter(baseWriter, 4);

            // Act
            using (writer.Indent())
            {
                writer.Write("asdf");
            }

            // Assert
            baseWriter.ToString().Should().Be("    asdf");
        }
    }
}