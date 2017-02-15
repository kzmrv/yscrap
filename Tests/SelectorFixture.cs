using FluentAssertions;
using NUnit.Framework;
using Parser.Commands.Selectors;

namespace Parser.Tests
{
    [TestFixture]
    class SelectorFixture
    {
        [Test]
        public void When_parsing_single_node_selector()
        {
            var pattern = ".//div[@class='review-node']//h1[@class='title']/";
            var selector = Selector.Create(pattern);
            selector.Should().BeOfType<SingleNodeSelector>();
        }
    }
}
