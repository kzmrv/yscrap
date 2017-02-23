using HtmlAgilityPack;

namespace Parser.Commands.Generators
{
    class AttributeGenerator : Generator
    {
        readonly string path;
        readonly string attribute;
        public AttributeGenerator(string path, string attribute)
        {
            this.path = path;
            this.attribute = attribute;
        }

        public override string GenerateFrom(HtmlNode node)
        {
            var realNode = node;
            if (!string.IsNullOrWhiteSpace(path))
            {
                realNode = node.SelectSingleNode(path);
            }
            return realNode.GetAttributeValue(attribute, null);
        }
    }
}
