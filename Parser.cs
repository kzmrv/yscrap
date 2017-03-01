using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
            return result;
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

        static void ProcessBlock(ScrapperBlock block, HtmlNode htmlNode, ExpandoObject objectNode)
        {
            object nextObjectNodes = objectNode;
            var nextHtmlNodes = new[] { htmlNode };

            // Create field or intermediate object
            if (block.create != null)
            {
                object contentToCreate;
                AssertNull(block.createArray, nameof(block.createArray));
                if (block.value != null)
                {
                    AssertNull(block.createArray, nameof(block.createArray));
                    AssertNull(block.children, nameof(block.children));
                    AssertNull(block.exactValue, nameof(block.exactValue));
                    AssertNull(block.select, nameof(block.select));
                    AssertNull(block.selectMany, nameof(block.selectMany));
                    contentToCreate = ProcessGenerator(htmlNode, block.value);
                }
                else
                {
                    AssertNotAllAreNull("children", block.children, block.exactValue);
                    contentToCreate = new ExpandoObject();
                }
                Attach(contentToCreate, objectNode, block.create);
                nextObjectNodes = contentToCreate;
            }

            if (block.createArray != null)
            {
                AssertNull(block.select, nameof(block.select));
                AssertNotNull(block.selectMany, nameof(block.selectMany));
                // array is created after processing multi-selector
            }

            if (block.select != null)
            {
                AssertNull(block.selectMany, nameof(block.selectMany));
                nextHtmlNodes = new[] { ProcessSingleSelector(htmlNode, block.select) };
            }

            if (block.selectMany != null)
            {
                nextHtmlNodes = ProcessMultiSelector(htmlNode, block.selectMany);

                // Process 'createArray' now
                var length = nextHtmlNodes.Length;
                var arrayToCreate = new ExpandoObject[nextHtmlNodes.Length];
                for (var i = 0; i < length; i++)
                {
                    arrayToCreate[i] = new ExpandoObject();
                }
                Attach(arrayToCreate, objectNode, block.createArray);
                nextObjectNodes = arrayToCreate;
            }

            if (block.exactValue != null)
            {
                AssertNull(block.children, nameof(block.children));
                if (nextObjectNodes is ExpandoObject)
                {
                    CreateAndAttachExact(block.exactValue, nextHtmlNodes[0], (ExpandoObject)nextObjectNodes);
                }
                if (nextObjectNodes is ExpandoObject[])
                {
                    var array = (ExpandoObject[])nextObjectNodes;
                    for (var i = 0; i < array.Length; i++)
                    {
                        array[i] = CreateExact(block.exactValue, nextHtmlNodes[i]);
                    }
                }

            }

            if (block.children != null)
            {
                if (nextObjectNodes is ExpandoObject)
                {
                    foreach (var child in block.children)
                    {
                        ProcessBlock(child, nextHtmlNodes[0], ((ExpandoObject)nextObjectNodes));
                    }
                }

                if (nextObjectNodes is ExpandoObject[])
                {
                    for (var i = 0; i < nextHtmlNodes.Length; i++)
                    {
                        foreach (var child in block.children)
                        {
                            ProcessBlock(child, nextHtmlNodes[i], ((ExpandoObject[])nextObjectNodes)[i]);
                        }
                    }
                }
            }
        }

        static void Attach(object what, object toWhat, string withName)
        {
            if (what == null)
            {
                return;
            }
            var objectNodeMembers = (IDictionary<string, object>)toWhat;
            objectNodeMembers.Add(withName, what);
        }


        static string ProcessGenerator(HtmlNode node, string generatorPattern)
        {
            var generator = Generator.Create(generatorPattern);
            return generator.GenerateFrom(node);
        }

        static HtmlNode ProcessSingleSelector(HtmlNode node, string selectorPattern)
        {
            var selector = SingleSelector.Create(selectorPattern);
            return selector.Execute(node);
        }

        static HtmlNode[] ProcessMultiSelector(HtmlNode node, string selectorPattern)
        {
            var selector = MultiSelector.Create(selectorPattern);
            return selector.Execute(node);
        }

        static void CreateAndAttachExact(ExpandoObject schemaObject, HtmlNode htmlNode, ExpandoObject attachTo)
        {
            var generatedFields = GeneratePropertiesExact(schemaObject, htmlNode);
            var parentFields = attachTo as IDictionary<string, object>;
            foreach (var generatedField in generatedFields)
            {
                parentFields.Add(generatedField.Key, generatedField.Value);
            }
        }

        static Dictionary<string, object> GeneratePropertiesExact(ExpandoObject schemaObject,
            HtmlNode htmlNode)
        {
            var resultFields = new Dictionary<string, object>();
            var schemaFields = schemaObject as IDictionary<string, object>;
            foreach (var property in schemaFields)
            {
                var innerProperties = property.Value as IDictionary<string, object>;

                var newField = (innerProperties == null)
                    ? (object)ProcessGenerator(htmlNode, property.Value as string)
                    : CreateExact(innerProperties as ExpandoObject, htmlNode);

                resultFields.Add(property.Key, newField);
            }
            return resultFields;
        }

        static ExpandoObject CreateExact(ExpandoObject schemaObject, HtmlNode htmlNode)
        {
            var properties = GeneratePropertiesExact(schemaObject, htmlNode);
            return properties.ToExpandoObject();
        }

        static void AssertNull(object value, string name = "")
        {
            if (value == null) return;
            throw new ArgumentException($"Expected element {name} to be null but was {value}");
        }

        static void AssertNotNull(object value, string name = "")
        {
            if (value != null) return;
            throw new ArgumentException($"Expected element {name} to be not null");
        }

        static void AssertNotAllAreNull(string where, params object[] objects)
        {
            if (objects.Any(obj => obj != null))
            {
                return;
            }
            throw new ArgumentException($"Expected at least some objects to be not null in {where}");
        }
    }

    public static class Extensions
    {
        public static ExpandoObject ToExpandoObject(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                var value = kvp.Value as IDictionary<string, object>;
                if (value != null)
                {
                    var expandoValue = value.ToExpandoObject();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        var objects = item as IDictionary<string, object>;
                        if (objects != null)
                        {
                            var expandoItem = objects.ToExpandoObject();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }

                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }

            return expando;
        }
    }
}
