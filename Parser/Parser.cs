using System;
using System.Collections.Generic;
using System.Dynamic;
using HtmlAgilityPack;

namespace Parser
{
    public class Parser
    {
        const string ParentAlias = "parent";
        readonly ParserSchema schema;

        public Parser(ParserSchema schema)
        {
            this.schema = schema;
        }

        public ExpandoObject Parse(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var result = Parse(document.DocumentNode);
            return Unloop(result);
        }

        static ExpandoObject Unloop(ExpandoObject objectNode)
        {
            var members = objectNode as IDictionary<string, object>;
            members.Remove(ParentAlias);
            foreach (var member in members)
            {
                var fieldValue = member.Value as ExpandoObject;
                if (fieldValue != null)
                {
                    Unloop(fieldValue);
                }
            }
            return objectNode;
        }

        ExpandoObject Parse(HtmlNode root)
        {
            var outputRoot = new ExpandoObject();
            var blocks = schema.children;
            foreach (var block in blocks)
            {
                ProcessBlock(block, root, outputRoot);
            }
            return outputRoot;
        }

        static void ProcessBlock(ScrapperBlock block, HtmlNode htmlNode, ExpandoObject parent)
        {
            var nextParent = parent;
            if (block.value != null)
            {
                var dict = parent as IDictionary<string, object>;
                if (block.create == null)
                {
                    throw new ArgumentException("Output cant be null here");
                }
                var value = ProcessGenerator(block.value, htmlNode);
                dict.Add(block.create, value);
            }
            else
            {
                if (block.create != null)
                {
                    nextParent = new ExpandoObject();
                    var nextParentMembers = (IDictionary<string, object>)nextParent;
                    nextParentMembers.Add(ParentAlias, parent);

                    var parentMembers = parent as IDictionary<string, object>;
                    parentMembers.Add(block.create, nextParent);
                }
            }

            var nextNode = htmlNode;
            if (block.@select != null)
            {
                nextNode = ProcessSelector(htmlNode, block.@select);
            }
            if (block.children == null) return;
            foreach (var child in block.children)
            {
                ProcessBlock(child, nextNode, nextParent);
            }
        }

        const string InnerTextSignature = "#";

        static string ProcessGenerator(string generator, HtmlNode node)
        {
            if (generator.EndsWith(InnerTextSignature))
            {
                var path = generator.Substring(0, generator.Length - InnerTextSignature.Length);
                var hasPath = !string.IsNullOrWhiteSpace(path);
                var finalNode = hasPath ? node.SelectSingleNode(path) : node;
                return finalNode.InnerText;
            }

            throw new ArgumentException("Invalid generator signature");
        }

        const string SelectSingleNodeSignature = "/";

        static HtmlNode ProcessSelector(HtmlNode current, string selector)
        {
            if (selector.EndsWith(SelectSingleNodeSignature))
            {
                var path = selector.Substring(0, selector.Length - SelectSingleNodeSignature.Length);
                return current.SelectSingleNode(path);
            }
            throw new ArgumentException("Unknown selector");
        }
    }
}
