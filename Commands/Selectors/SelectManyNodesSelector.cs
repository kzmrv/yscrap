using System.Linq;
using HtmlAgilityPack;

namespace Parser.Commands.Selectors
{
    public class SelectManyNodesSelector : MultiSelector
    {
        readonly string path;

        public SelectManyNodesSelector(string path)
        {
            this.path = path;
        }

        public override HtmlNode[] Execute(HtmlNode currentNode)
        {
            return currentNode.SelectNodes(path).ToArray();
        }
    }
}
