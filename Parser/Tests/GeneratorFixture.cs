using FluentAssertions;
using NUnit.Framework;
using Parser.Commands.Generators;

namespace Parser.Tests
{
    [TestFixture]
    public class GeneratorFixture
    {
        [Test]
        public void When_parsing_inner_text_generator_simple()
        {
            var pattern = ".//a/span#";
            var generator = Generator.Create(pattern);
            generator.Should().BeOfType<InnerTextGenerator>();
        }
    }
}
