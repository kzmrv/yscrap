using System;
using HtmlAgilityPack;

namespace Parser.Commands.Selectors
{
    public abstract class Selector
    {
        public abstract HtmlNode[] Execute(HtmlNode currentNode);

        public static Selector Create(string pattern)
        {
            if (pattern.EndsWith("/")) return new SingleNodeSelector(pattern);
            throw new ArgumentException("Unknown selector pattern");
        }
    }
}
