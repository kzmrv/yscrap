using FluentAssertions;
using NUnit.Framework;
using Parser.Commands.Selectors;

namespace Parser.Tests
{
    [TestFixture]
    public class SelectorsFixture
    {
        [Test]
        public void When_parsing_single_node_selector()
        {
            var pattern = ".//div[@class='review-node']//h1[@class='title']/";
            var selector = SingleSelector.Create(pattern);
            selector.Should().BeOfType<SelectSingleNodeSelector>();
        }

        [Test]
        public void When_parsing_multiple_node_selector()
        {
            var pattern = "./div[@class='review-node']/h1[@class='title']//";
            var selector = MultiSelector.Create(pattern);
            selector.Should().BeOfType<SelectManyNodesSelector>();
        }
    }
}
