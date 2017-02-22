﻿using HtmlAgilityPack;

namespace Parser.Commands.Selectors
{
    public class SingleNodeSelector : Selector
    {
        readonly string path;

        public SingleNodeSelector(string path)
        {
            this.path = path;
        }

        public override HtmlNode[] Execute(HtmlNode currentNode)
        {
            return new[] { currentNode.SelectSingleNode(path) };
        }
    }
}
