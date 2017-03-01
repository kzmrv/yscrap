using System;
using HtmlAgilityPack;

namespace Parser.Commands.Selectors
{
    public abstract class MultiSelector
    {
        public const string MultipleNodeEnding = "//";
        public static MultiSelector Create(string pattern)
        {
            if (pattern.EndsWith("//"))
            {
                return new SelectManyNodesSelector(pattern.Substring(0, pattern.Length - MultipleNodeEnding.Length));
            }
            throw new ArgumentException($"Invalid selector pattern: {pattern}");
        }

        public abstract HtmlNode[] Execute(HtmlNode currentNode);
    }
}
