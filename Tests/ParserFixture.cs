using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace Parser.Tests
{
    [TestFixture]
    public class ParserFixture
    {
        static readonly string directory = TestContext.CurrentContext.TestDirectory;
        static string ReadFile(string path) => File.ReadAllText(Path.Combine(directory, path));

        static ExpandoObject Parse(string schemaPath, string sourcePath)
        {
            var schemaJson = ReadFile(Path.Combine("Tests//Data", schemaPath));
            var source = ReadFile(Path.Combine("Tests//Data", sourcePath));
            var schema = ParserSchema.FromJson(schemaJson);
            var parser = new Parser(schema);
            return parser.Parse(source);
        }

        [Test]
        public void When_parsing_some_simple_html()
        {
            var obj = Parse("simple_schema.json",
                "book_example.html")
                as IDictionary<string, object>;
            obj.Should().ContainKey("title");
            obj["title"].Should().BeOfType<ExpandoObject>();
            var title = (ExpandoObject)obj["title"];
            title.Should().ContainKey("name");
        }

        [Test]
        public void When_parsing_schema_with_attribute_generator()
        {
            dynamic obj = Parse("schema_with_attribute.json",
                "book_example.html");
            ((string)obj.title.name).Should().BeEquivalentTo("valueId");
        }
    }
}
