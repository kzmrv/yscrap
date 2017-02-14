using HtmlAgilityPack;

namespace Parser.Commands.Selectors
{
    public class SingleNodeSelector : Selector
    {
        const string SingleNodeSelectorSignature = "/";
        readonly string pattern;

        public SingleNodeSelector(string pattern)
        {
            this.pattern = pattern;
        }

        public override HtmlNode[] Execute(HtmlNode currentNode)
        {
            var path = pattern.Substring(0, pattern.Length - SingleNodeSelectorSignature.Length);
            return new[] { currentNode.SelectSingleNode(path) };
        }
    }
}
