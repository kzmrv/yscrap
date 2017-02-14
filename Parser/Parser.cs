using System;
using System.Collections.Generic;
using System.Dynamic;
using HtmlAgilityPack;
using Parser.Commands.Generators;
using Parser.Commands.Selectors;

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
                    throw new ArgumentException("Output can't be null here");
                }
                var value = ProcessGenerator(htmlNode, block.value);
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

            var nextNodes = new[] { htmlNode };
            if (block.select != null)
            {
                nextNodes = ProcessSelector(htmlNode, block.select);
            }

            foreach (var child in block.children)
            {
                foreach (var nextNode in nextNodes)
                {
                    ProcessBlock(child, nextNode, nextParent);
                }
            }
        }

        static string ProcessGenerator(HtmlNode node, string generatorPattern)
        {
            var generator = Generator.Create(generatorPattern);
            return generator.Generate(node);
        }

        static HtmlNode[] ProcessSelector(HtmlNode node, string selectorPattern)
        {
            var selector = Selector.Create(selectorPattern);
            return selector.Execute(node);
        }


    }
}
