using HtmlAgilityPack;

namespace Parser.Commands.Selectors
{
    public class SelectSingleNodeSelector : SingleSelector
    {
        readonly string path;

        public SelectSingleNodeSelector(string path)
        {
            this.path = path;
        }

        public override HtmlNode Execute(HtmlNode currentNode)
        {
            return currentNode.SelectSingleNode(path);
        }
    }
}
