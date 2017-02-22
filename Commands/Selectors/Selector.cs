using System;
using HtmlAgilityPack;

namespace Parser.Commands.Selectors
{
    public abstract class Selector
    {
        public abstract HtmlNode[] Execute(HtmlNode currentNode);

        public static Selector Create(string pattern)
        {
            const string SingleNodeEnding = "/";
            const string MultipleNodeEnding = "//";
            if (pattern.EndsWith(SingleNodeEnding) && !pattern.EndsWith(MultipleNodeEnding))
            {
                return new SingleNodeSelector(pattern.Substring(0, pattern.Length - SingleNodeEnding.Length));
            }
            if (pattern.EndsWith("//"))
            {
                return new MultipleNodeSelector(pattern.Substring(0, pattern.Length - MultipleNodeEnding.Length));
            }
            throw new ArgumentException("Unknown selector pattern");
        }
    }
}
