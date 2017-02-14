using HtmlAgilityPack;

namespace Parser.Commands.Generators
{
    public class InnerTextGenerator : Generator
    {
        public const string InnerTextGeneratorSignature = "#";

        readonly string pattern;

        public InnerTextGenerator(string pattern)
        {
            this.pattern = pattern;
        }

        public override string Generate(HtmlNode node)
        {
            var path = pattern.Substring(0, pattern.Length - InnerTextGeneratorSignature.Length);
            var hasPath = !string.IsNullOrWhiteSpace(path);
            var finalNode = hasPath ? node.SelectSingleNode(path) : node;
            return finalNode.InnerText;
        }
    }
}
