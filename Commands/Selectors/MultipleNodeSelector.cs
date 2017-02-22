using System.Linq;
using HtmlAgilityPack;

namespace Parser.Commands.Selectors
{
    public class MultipleNodeSelector : Selector
    {
        readonly string path;

        public MultipleNodeSelector(string path)
        {
            this.path = path;
        }

        public override HtmlNode[] Execute(HtmlNode currentNode)
        {
            return currentNode.SelectNodes(path).ToArray();
        }
    }
}
