using System;
using HtmlAgilityPack;

namespace Parser.Commands.Selectors
{
    public abstract class SingleSelector
    {
        const string SingleNodeEnding = "/";
        public static SingleSelector Create(string pattern)
        {
            if (pattern.EndsWith(SingleNodeEnding) && !pattern.EndsWith(MultiSelector.MultipleNodeEnding))
            {
                return new SelectSingleNodeSelector(pattern.Substring(0, pattern.Length - SingleNodeEnding.Length));
            }
            throw new ArgumentException($"Invalid selector pattern: {pattern}");
        }

        public abstract HtmlNode Execute(HtmlNode currentNode);
    }
}
